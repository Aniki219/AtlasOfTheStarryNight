using UnityEngine;
using System;
using System.Collections;
using MyBox;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ColliderManager), typeof(StateMachine))]
public class characterController : MonoBehaviour
{
    #region Defs
    [Foldout("Debug", true)] public bool debug = false;
    [ConditionalField(nameof(debug))] public bool showNormal = true;
    [ConditionalField(nameof(debug))] public bool showVelocityNormal = false;
    [ConditionalField(nameof(debug))] public bool showCollisionResolution = false;
    [ConditionalField(nameof(debug))] public bool  highlightGrounded = false;
    #if UNITY_EDITOR // conditional compilation is not mandatory
    [ButtonMethod]
    private void ToggleDebug()
    {
        debug = !debug;
    }
    #endif


    [Foldout("Velocity")] public Vector3 velocity;
    public Vector3 velocityOut { get; private set;}
    [Foldout("Velocity")] [SerializeField] Vector3 additionalVelocity;
    [Foldout("Velocity")] public bool lockPosition = false;
    [Foldout("Velocity")] public bool canMove = true;
    [Foldout("Velocity")] public bool canGravity = true;

    [Foldout("Collision Info")] public CollisionState collisions;

    [Foldout("Collision Info")] public LayerMask collisionMask;
    [Foldout("Collision Info")] public LayerMask dangerMask;

    [Foldout("Special")] public bool canCrouch = false;

    //TODO: wtf is this?
    private float safetyMargin = 1;

    private int horizontalRayCount = 15;
    private int verticalRayCount = 8;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    
    [Foldout("Gravity")] public float termVel = -10;
    [Foldout("Gravity")] public float gravityMod = 1.0f;
    public float gravity {get; private set;}
    [Foldout("Gravity")] [SerializeField] private float currentGravity = 0;
    [Foldout("Gravity")] public float maxGravity = 8.0f;
    private float gravitySmoothing = 0;

    [Foldout("Gravity")] public float maxSlope = 0.5f;

    StateMachine stateMachine;
    ColliderManager colliderManager;
    BoundaryPoints boundaryPoints;

    [HideInInspector] public Vector3 cameraTarget;

    #endregion

    #region Events
     public bool bonkCeiling = false;
     [ConditionalField("bonkCeiling")] public UnityEvent OnBonkCeiling;


     public bool hasLandingEvent = false;
     [ConditionalField("hasLandingEvent")] public UnityEvent OnLanding;

    #endregion

    void Start()
    {
        gravity = gameManager.Instance.gravity;
        colliderManager = GetComponent<ColliderManager>();
        stateMachine = GetComponent<StateMachine>();
        collisions.Init();
    }

    private void FixedUpdate()
    {
        //Keep track of "justLanded" and "justHeadBonked"
        UpdateBoundaryPoints();
        if (canMove)
        {
            Move(velocity);

            if (canGravity)
            {
                doGravity();
            } else
            {
                resetGravity();
            }
        }
    }

    private void LateUpdate() {
        BoxCollider2D collider = colliderManager.getCollider();
        if (debug) {
            UpdateBoundaryPoints();
            GetComponentInChildren<SpriteRenderer>().material.color = Color.white.WithAlphaSetTo(0.75f);
            if (showCollisionResolution) debugBoundaryCollisions();
            if (showNormal) {
                foreach(Vector2 normal in collisions.getNorms()) {
                    Debug.DrawLine(collider.bounds.center, collider.bounds.center + (Vector3)normal, Color.magenta);
                }
                Debug.DrawLine(collider.bounds.center, collider.bounds.center + (Vector3)collisions.getAverageNorm(), Color.red);
            }
            if (showVelocityNormal) {
                Debug.DrawLine(collider.bounds.center, collider.bounds.center + velocity.normalized, Color.grey);
                Debug.DrawLine(collider.bounds.center, collider.bounds.center + velocityOut.normalized, Color.white);
            }
        }
    }

    private void doGravity()
    {
        if (collisions.isGrounded() && collisions.getGroundSlope().y >= maxSlope)
        {
            currentGravity = 0;
            velocity.y = Mathf.Max(velocity.y, 0);
        } else
        {
            currentGravity = gravity * Time.fixedDeltaTime * gravityMod  * (velocity.y > 0 ? 1 : 1.25f);
            velocity.y = Mathf.Max(velocity.y, termVel);
        }

        velocity.y += currentGravity;
    }

    public enum VelocityType {
        Relative,
        Absolute,
        Reverse
    }
    public void setXVelocity(float value, VelocityType vtype = VelocityType.Relative) {
        velocity.x = value;
        if (vtype.Equals(VelocityType.Relative)) velocity.x *= stateMachine.facing;
        if (vtype.Equals(VelocityType.Reverse)) velocity.x *= -stateMachine.facing;
    }

    public void resetGravity()
    {
        currentGravity = 0;
    }

    public void Move(Vector3 vel) {
        vel = (vel + additionalVelocity) * Time.deltaTime;

        if (collisions.hasNormWhere(norm => norm.y > 0) && 
            collisions.hasNormWhere(norm => norm.y < 0.75f) && 
            vel.y <= 0) 
        {
            vel *= 0.8f;
        }

        if (lockPosition) { return; }

        collisions.Reset();

        Vector3 oldPosition = transform.position * 1.0f;

        transform.position += (Vector3)resolveCollision(vel);
        checkGrounded();

        if (collisions.wasGrounded && !collisions.isGrounded() && vel.y <= 0) {
            CollisionData checkDown = detectCollision(slopeDetectRange * Time.deltaTime * Vector2.down); 
            if (checkDown.hit) {
                float dist = Mathf.Abs(checkDown.distance);
                transform.position += (Vector3)resolveCollision(dist * Vector3.down, false);
                checkGrounded();
            }
        }

        velocityOut = transform.position - oldPosition;
    }
    [Range(5, 100)]
    public float slopeDetectRange = 5;

    private Vector2 resolveCollision(Vector2 vel, bool canSlide = true) {
        Vector2 dp = vel;

        int tries = 0;
        while(tries <= 8) { 
            CollisionData collisionData = detectCollision(dp);

            if (!canSlide) {
                collisionData.normal = Vector2.up;
            }

            if (collisionData.hit) {
                dp += collisionData.normal * (Mathf.Abs(collisionData.distance) + 0.002f);
                if (Vector2.Dot(vel, collisionData.normal) > 0) {
                    dp = collisionData.normal * vel.magnitude;
                }
            } else {
                return dp;
            }
            tries++;
        }
        return Vector2.zero;
    }

    public struct CollisionData {
        public bool hit;
        public Vector2 normal;
        public float distance;
        public Collider2D collider;
        public Collider2D otherCollider;
    }

    public CollisionData detectCollision(Vector2 dp) {
        BoxCollider2D collider = colliderManager.getCollider();

        Vector2 originalOffset = collider.offset;
        Collider2D boxhitCollider;

        // float maxStep = collider.size.x/2.0f;
        // int stepsNeeded = dp / maxStep;
        // for (int i = ) {
            collider.offset = originalOffset + dp;

            boxhitCollider = Physics2D.OverlapBox(
                (Vector2)transform.position + collider.offset, 
                Vector2.Scale(collider.size, (Vector2)transform.localScale), 
                0, 
                collisionMask
            );

        //     if (boxhitCollider) break;
        // }

        CollisionData returnData = new CollisionData();

        if (boxhitCollider) {
            ColliderDistance2D colliderDistance = boxhitCollider.Distance(collider);
            
            returnData.hit = true;
            returnData.distance = colliderDistance.distance;
            returnData.normal = new Vector2(Mathf.Round(colliderDistance.normal.x * 100f)/100f, Mathf.Round(colliderDistance.normal.y * 100f)/100f);
            
            returnData.collider = collider;
            returnData.otherCollider = boxhitCollider;


            collisions.setCollisionInfo(returnData);

            if (Mathf.Approximately(returnData.normal.y, -1) && velocity.y > 0) {
                OnBonkCeiling.Invoke();
            } 

            //Land on slopes as if they were horizontal
            if (returnData.normal.y >= maxSlope) {
                returnData.normal = Vector2.up;
            }

            //Walk into steep walls as if they were vertical ->/ => ->|
            // if (collisions.wasGrounded && returnData.normal.y < maxSlope && Mathf.Sign(returnData.normal.x) != Mathf.Sign(velocity.x) ) {
            //     returnData.normal.y = 0;
            //     returnData.normal.Normalize();
            // }

        }

        collider.offset = originalOffset;
        return returnData;
    }

    public CollisionData checkGrounded()
    {
        CollisionData data = detectCollision(Vector2.up * ( - groundedCheckRange));
        if (!collisions.isGrounded() && data.hit) OnLanding.Invoke();
        if (velocity.y <= 0) collisions.setGroundSlope(data.normal);
        collisions.setGrounded(velocity.y <= 0 && data.hit);

        return data;
    }
    [Range(0.002f, 0.020f)]
    public float groundedCheckRange = 0.008f;

    public bool isGrounded() {
        return collisions.isGrounded();
    }

    public void AddVelocity(Vector3 amount)
    {
        if (!collisions.isTangible()) { return; }
        additionalVelocity += amount;
    }

    public bool checkVertDist(float dist)
    {
        return !detectCollision(Vector2.up * dist).hit;
    }

#region Rework needed
    bool crushTest(bool vertical)
    {
        // float rayLength = 4.0f / 32.0f;
        // Vector3 dir;
        // Vector2 rayOrigin1, rayOrigin2, rayDirection;
        // float raySpacing;
        // int rayCount;

        // if (vertical)
        // {
        //     dir = Vector3.up;

        //     rayCount = verticalRayCount;

        //     rayOrigin1 = boundaryPoints.topLeft;
        //     rayOrigin2 = boundaryPoints.bottomLeft;

        //     raySpacing = verticalRaySpacing;
        //     rayDirection = Vector2.right;
        // } else
        // {
        //     dir = Vector3.right;

        //     rayCount = horizontalRayCount;

        //     rayOrigin1 = boundaryPoints.bottomRight;
        //     rayOrigin2 = boundaryPoints.bottomLeft;

        //     raySpacing = horizontalRaySpacing;
        //     rayDirection = Vector2.up;
        // }

        // for (int i = 0; i < rayCount; i++)
        // {
        //     RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin1, dir, rayLength, collisionMask);
        //     RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, -dir, rayLength, collisionMask);

        //     //if (hit1) {
        //     //    Debug.DrawLine(rayOrigin1, rayOrigin1 + (Vector2)dir * rayLength, Color.red);
        //     //} else
        //     //{
        //     //    Debug.DrawLine(rayOrigin1, rayOrigin1 + (Vector2)dir * rayLength, Color.green);
        //     //}
        //     //if (hit2) {
        //     //    Debug.DrawLine(rayOrigin2, rayOrigin2 + -(Vector2)dir * rayLength, Color.red);
        //     //} else
        //     //{
        //     //    Debug.DrawLine(rayOrigin2, rayOrigin2 + -(Vector2)dir * rayLength, Color.green);
        //     //}

        //     if (hit1 && hit2 && !(hit1.transform.gameObject.Equals(hit2.transform.gameObject)))
        //     {
        //         return true;
        //     }

        //     rayOrigin1 += rayDirection * (raySpacing);
        //     rayOrigin2 += rayDirection * (raySpacing);
        // }
        return false;
    }

    public bool isSafePosition()
    {
        BoxCollider2D collider = colliderManager.getCollider();

        if (!collisions.isTangible()) { return false; }
        Vector3 boxCastOrigin = collider.transform.position - Vector3.right * safetyMargin/2 - collider.size.y/2 * Vector3.up;

        //Debug.DrawLine((Vector2)boxCastOrigin + collider.size.y / 2 * Vector2.up, (Vector2)boxCastOrigin + (safetyMargin + collider.size.x/2) * Vector2.right - collider.size.y / 2 * Vector2.up);
        //Debug.DrawLine((Vector2)boxCastOrigin - collider.size.y / 2 * Vector2.up, (Vector2)boxCastOrigin + (safetyMargin + collider.size.x/2) * Vector2.right + collider.size.y / 2 * Vector2.up);

        LayerMask safeGroundMask = collisionMask & ~(LayerMask.GetMask("DesctructibleBlock"));

        if (Physics2D.Raycast(transform.position, Vector2.right, 0.4f, safeGroundMask)) return false;
        if (Physics2D.Raycast(transform.position, -Vector2.right, 0.4f, safeGroundMask)) return false;
        if (Physics2D.BoxCast(boxCastOrigin, collider.size, 0, Vector3.right, safetyMargin, dangerMask)) return false;

        int rays = 0;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = boundaryPoints.bottomLeft + (Vector2.right * verticalRaySpacing * i);
            float maxDistance = 1 / 32f;// + skinWidth;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, maxDistance, safeGroundMask);

            if (hit)
            {
                rays++;
            }
        }
        return rays > 2;
    }
#endregion

    public void UpdateBoundaryPoints()
    {
        Bounds bounds = colliderManager.getCollider().bounds;

        boundaryPoints.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        boundaryPoints.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        boundaryPoints.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        boundaryPoints.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    struct BoundaryPoints
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    [Serializable]
    public struct CollisionState
    {
        private bool above, below;
        private bool left, right;

        [SerializeField] private List<Vector2> normals;
        [SerializeField] private Vector2 groundSlope;
        private Vector2 closestMidPoint;
        private Vector2 closestFootPoint;
        private Vector2 closestHeadPoint;
        [SerializeField] private bool grounded;
        public bool wasGrounded {get; private set;}

        [SerializeField] private bool tangible;
        [SerializeField] private bool collidable;

        public void Reset(bool resetBelow = true)
        {
            above = false;
            below = false;
            left = false;
            right = false;
            normals = new List<Vector2>();
            closestMidPoint = Vector2.zero;
            closestFootPoint = Vector2.zero;
            closestHeadPoint = Vector2.zero;
            wasGrounded = grounded;
            grounded = false;
        }

        public void Init()
        {
            Reset();
            tangible = true;
            collidable = true;
        }

        public void setCollisionInfo(CollisionData collisionData) {
            if (!normals.Contains(collisionData.normal)) {
                normals.Add(collisionData.normal);
            }

            foreach(Vector2 normal in normals) {
                if (normal.x > 0) left = true;
                if (normal.x < 0) right = true;
                if (normal.y > 0) below = true;
                if (normal.y < 0) above = true;
            }

            //setGroundSlope(collisionData.normal);

            Collider2D collider = collisionData.collider;
            closestMidPoint = collisionData.otherCollider.ClosestPoint(collider.bounds.center);
            closestHeadPoint = collisionData.otherCollider.ClosestPoint((Vector2)collider.bounds.max + collider.bounds.extents.x * Vector2.left);
            closestFootPoint = collisionData.otherCollider.ClosestPoint((Vector2)collider.bounds.min + collider.bounds.extents.x * Vector2.right);
        }

        public bool isGrounded() {
            return grounded;
        }

        public void setGrounded(bool isGrounded) {
            grounded = isGrounded;
        }

        public void setGroundSlope(Vector2 slopeNormal) {
            groundSlope = slopeNormal;
        }

        public Vector2 getGroundSlope() {
            return groundSlope;
        }

        public Vector2 getPoint() {
            return closestMidPoint;
        }

        public bool getLeft() {
            return left;
        }

        public bool getRight() {
            return right;
        }

        public bool getAbove() {
            return above;
        }

        public bool getBelow() {
            return below;
        }

        public List<Vector2> getNorms() {
            return normals;
        }

        public Vector2 getAverageNorm() {
            return (normals.Aggregate(Vector2.zero, (v1, v2) => v1 + v2) / normals.Count).normalized;
        }

        public bool hasNormWhere(Predicate<Vector2> lambda) {
            return normals.Find(lambda) != default;
        }

        public void setCollidable(bool on = true) {
            collidable = on;
        }

        public void setTangible(bool on = true) {
            tangible = on;
        }

        public bool isCollidable() {
            return collidable;
        }

        public bool isTangible() {
            return tangible;
        }

        public Vector2 getHeadPoint() {
            return closestHeadPoint;
        }

        public Vector2 getMidPoint() {
            return closestMidPoint;
        }

        public Vector2 getFootPoint() {
            return closestFootPoint;
        }
    }

    public void debugBoundaryCollisions() {
        Debug.DrawLine(boundaryPoints.bottomLeft, boundaryPoints.bottomRight, collisions.getBelow() ? Color.green : Color.white);
        Debug.DrawLine(boundaryPoints.bottomLeft, boundaryPoints.topLeft, collisions.getLeft() ? Color.green : Color.white);
        Debug.DrawLine(boundaryPoints.bottomRight, boundaryPoints.topRight, collisions.getRight() ? Color.green : Color.white);
        Debug.DrawLine(boundaryPoints.topLeft, boundaryPoints.topRight, collisions.getAbove() ? Color.green : Color.white);
    }
}