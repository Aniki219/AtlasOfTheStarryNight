using UnityEngine;
using System.Collections;
using MyBox;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
public class characterController : MonoBehaviour
{
    #region Defs
    public LayerMask collisionMask;
    public LayerMask dangerMask;

    public bool lockPosition = false;

    //TODO: wtf is this?
    private float safetyMargin = 1;

    private int horizontalRayCount = 6;
    private int verticalRayCount = 6;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    public float termVel = -10;
    public float gravityMod = 1.0f;
    private float gravity;
    private float currentGravity = 0;
    private float maxGravity = 10.0f;

    new BoxCollider2D collider;
    BoundaryPoints boundaryPoints;
    
    Vector3 additionalVelocity;

    public CollisionInfo collisions;
    public Vector3 velocity;
    public Vector3 cameraTarget;

    public bool canMove = true;
    public bool canGravity = true;
    #endregion

    #region Events
    public bool bonkCeiling = false;
    [ConditionalField("bonkCeiling")] public BonkCeilingEvent OnBonkCeiling;
    [System.Serializable]
    public class BonkCeilingEvent : UnityEvent<float> { }

    public bool hasLandingEvent = false;
    [ConditionalField("bonkCeiling")] public MyBoolEvent OnLanding;

    [System.Serializable]
    public class MyBoolEvent : UnityEvent<bool> {}
    #endregion

    void Start()
    {
        gravity = gameManager.Instance.gravity;
        collider = GetComponent<BoxCollider2D>();
        collisions.Init();
        CalculateRaySpacing();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();

            if (canGravity)
            {
                doGravity();
            } else
            {
                resetGravity();
            }
        }
        doMove(Vector2.right, true, true);
        //doMove(Vector2.left * AtlasConstants.StepSize * 5, true, false);
    }

    public void doGravity()
    {
        if (collisions.isGrounded)
        {
            currentGravity = 0;
            velocity.y = Mathf.Max(velocity.y, 0);
            
        } else
        {
            currentGravity = gravity * Time.fixedDeltaTime * gravityMod  * (velocity.y > 0 ? 1 : 1.5f);
            velocity.y = Mathf.Max(velocity.y, termVel);
        }

        velocity.y += currentGravity;    
    }

    public void resetGravity()
    {
        currentGravity = 0;
    }

    public void Move(Vector3 vel, bool canSlide = true)
    {
        doMove(vel * Time.fixedDeltaTime, canSlide);
    }

    public void Move(bool canSlide = true)
    {
        doMove(velocity * Time.fixedDeltaTime, canSlide);
    }

    bool test = false;
    private void doMove(Vector3 vel, bool canSlide, bool test = false) {
        if (lockPosition) { return; }

        //These should maybe be in Update()/FixedUpdate()
        UpdateBoundaryPoints();
        collisions.Reset();
        collisions.velocityOld = vel; // I dont know if we need this

        /* We can get the slope of the ground we are standing on while
         * checking if we even are grounded
         * */
        Vector2? groundSlope = checkGrounded();

        if (test)
        {
            //canSlide = false;
            vel = new Vector2(0, -2);
        }

        #region Modulate speed when walking up/down slopes and prevent steep slopes

        // If groundSlope has no value then we arent grounded
        if (groundSlope.HasValue && groundSlope.Value.y > 0)
        {
            Vector2 slope = groundSlope.Value;

            //I used Sign here to figure out if we are walking up a slope or down
            if (AtlasHelpers.Sign(slope.x) == AtlasHelpers.Sign(vel.x))
            {
                //We increase speed up slopes to keep movement feeling smooth and constant
                vel.x *= Vector2.Dot(slope, Vector2.up);
            } else
            {
                float dot = Vector2.Dot(slope, -vel.normalized);

                //This is probably wrong. Hand-waived guess at maximizing slope angle
                if (dot < 0.8f)
                {
                    vel /= Mathf.Sin(Mathf.Acos(dot));
                } else
                {
                    vel.x = 0;
                }
            }
        }
        #endregion

        #region Initialize desired and resulting vectors
        Vector2 desiredDisplacementVector = vel + additionalVelocity;
        Vector2 desiredDirection = desiredDisplacementVector.normalized;
        Vector2 resultingDisplacementVector = new Vector2(desiredDisplacementVector.x, desiredDisplacementVector.y);

        //Vector2 slopeOffset = Vector2.zero;
        #endregion

        #region Initialize all origins and thier offsets
        /*We need to figure out which sides of the collider should cast rays
         * We could use every side, but I think we can save on computation and just
         * use the sides that correspond to the x and y directions
         */
        List<AtlasRaycast> raycasts = calculateRaycastOrigins(desiredDisplacementVector);

        if (raycasts.Count == 0) return;
        #endregion

        /* Now we:
         * fire out the raycast
         * find the smallest ray
         * perform a slide calculation if allowed
         * do a step up check if for corners
         * perform another check for walking down slopes
         * translate the character
         */
        int tries = 0;
        float desiredDistance = desiredDisplacementVector.magnitude;
        while (desiredDistance > 0 && tries < 100) {

            #region Prevent infinite loop
            tries++;
            if (tries == 99)
            {
                Debug.LogError("max tries reached");
                break;
            }
            #endregion

            #region Find minimum distance raycast and record hit meta data in the ARC

            foreach (AtlasRaycast raycast in raycasts)
            {
                raycast.setCastVectorLength(desiredDistance);
                raycast.cast();
            }
            raycasts.Sort();
            AtlasRaycast minRaycast = raycasts[0];
            #endregion


            //Now we can use the meta data within the ARC to calculate a resulting vector
            resultingDisplacementVector = minRaycast.calcResultant();

            minRaycast.drawVectors();

            /*Here we perform a boxcast to check if the resulting location is free
             * This collider is halfway between the full box collider and the box emitting the
             * ARCs.
             * We may perform some correction logic if the location is invalid, otherwise
             * we repeat this entire process again with a AtlasConstants.StepSize shorter desiredVector
             */
            Vector2 desiredPosition = (Vector2)transform.position + resultingDisplacementVector;
            Vector2 colliderDesiredPosition = desiredPosition + collider.offset;

            RaycastHit2D boxHit = Physics2D.BoxCast(colliderDesiredPosition, 
                                                    collider.size - Vector2.one * AtlasConstants.StepSize, 
                                                    0, Vector2.zero, 0, collisionMask);

            //If the boxcast didnt hit anything we're good to go, we can exit the loop
                //AtlasHelperFunctions.DebugDrawBox(colliderDesiredPosition, collider.size - Vector2.one * AtlasConstants.StepSize, Color.green);
            if (!boxHit)
            {
                break;
            }
            //If we are just barely clipped by a collider such that oour raycast offset prevents us from seeing it
            //we can try to step up a tiny bit and see if that fixes the issue
            //Also if Our ray isn't long enough to detect a slope we are colliding with, we can just try to move up
            if (minRaycast.isCorner && minRaycast.incident.magnitude > AtlasConstants.StepSize && resultingDisplacementVector.y >= 0)
            {
                Vector2 o = minRaycast.originOffset;
                Vector2 v = minRaycast.incident;
                Vector2 v_hat = v.normalized;
                Vector2 o_parallel = Vector2.Dot(o, v) * -v_hat;
                Vector2 perpendicular = o - o_parallel;
                RaycastHit2D adjustmentHit = Physics2D.Raycast(minRaycast.origin + resultingDisplacementVector, -perpendicular.normalized, perpendicular.magnitude, collisionMask);
                if (adjustmentHit.collider != null || true)
                {
                    Vector2 a = -perpendicular.normalized * adjustmentHit.distance;
                    Vector2 stepUp = perpendicular + a;
                    Vector2 stepBack = v - Mathf.Sqrt(v.sqrMagnitude - stepUp.sqrMagnitude) * v_hat;
                    if (!Physics2D.BoxCast(colliderDesiredPosition + stepUp + stepBack, collider.size - Vector2.one * AtlasConstants.StepSize, 0, Vector2.zero, 0, collisionMask))
                    {
                        resultingDisplacementVector += stepUp + stepBack;
                        break;
                    }
                }
            }

            //If we made it this far without hitting a break, then we failed to find a safe spot and must try again
            //with a shorter desired displacement
            resultingDisplacementVector = Vector2.zero;
            desiredDistance = Mathf.Max(desiredDistance - AtlasConstants.StepSize, 0);
        }

        if (!test)
        {
            transform.Translate(resultingDisplacementVector);

            //After moving, if we were on the ground then we can try to follow a slope downwards up to ~45 degrees
            //TODO: Maybe we can put logic in here to look at the ground slope and determine how far to check
            if (collisions.isGrounded && canSlide)
            {
                //Maybe you have to hit something? I think you dont get to just go down for free
                //Seems okay for now though...
                doMove(Vector2.down * (resultingDisplacementVector.magnitude + AtlasConstants.StepSize*2), false);
            }
        }

        //I have no idea if this is correct at all
        cameraTarget = velocity/Time.deltaTime;

        if ((additionalVelocity.y != 0 && crushTest(true)) || (additionalVelocity.x != 0 && crushTest(false)))
        {
            playerController pc = GetComponent<playerController>();
            if (pc)
            {
                pc.startBonk(1, true);
            }
        }

        additionalVelocity = Vector3.zero;

        checkGrounded();
    }

    private List<AtlasRaycast> calculateRaycastOrigins(Vector2 cast)
    {
        List<AtlasRaycast> raycasts = new List<AtlasRaycast>();

        if (cast.x != 0)
        {
            //Get all of the raycast origins on the left or right side of the collider
            Vector2 origin = (cast.x > 0) ? boundaryPoints.bottomRight : boundaryPoints.bottomLeft;
            for (int i = 0; i < horizontalRayCount; i++)
            {
                bool isCorner = false;
                Vector2 originOffset = new Vector2(-AtlasConstants.StepSize * Mathf.Sign(cast.x), 0);
                if (i == 0 || i == horizontalRayCount - 1)
                {
                    //Corners are able to try for a "step up" correction so we mark them here
                    isCorner = true;
                    originOffset = new Vector2(-AtlasConstants.StepSize * Mathf.Sign(cast.x),
                                                (i == 0) ? AtlasConstants.StepSize : -AtlasConstants.StepSize);
                }

                AtlasRaycast raycast = AtlasRaycast.builder()
                    .withCastVector(cast)
                    .withOrigin(origin + i * horizontalRaySpacing * Vector2.up)
                    .withOriginOffset(originOffset)
                    .withIsCorner(isCorner)
                    .withCollisionMask(collisionMask)
                    .build();
                
                raycasts.Add(raycast);
            }
        }
        if (cast.y != 0)
        {
            //Get all of the raycast origins on the left or right side of the collider
            Vector2 origin = (cast.y > 0) ? boundaryPoints.topLeft : boundaryPoints.bottomLeft;
            for (int i = 0; i < verticalRayCount; i++)
            {
                bool isCorner = false;
                Vector2 originOffset = new Vector2(0, -AtlasConstants.StepSize * Mathf.Sign(cast.y));
                if (i == 0 || i == verticalRayCount - 1)
                {
                    //Corners are able to try for a "step up" correction so we mark them here
                    isCorner = true;
                    originOffset = new Vector2((i == 0) ? AtlasConstants.StepSize : -AtlasConstants.StepSize,
                                            -AtlasConstants.StepSize * Mathf.Sign(cast.y));
                }
                AtlasRaycast raycast = AtlasRaycast.builder()
                    .withCastVector(cast)
                    .withOrigin(origin + i * verticalRaySpacing * Vector2.right)
                    .withOriginOffset(originOffset)
                    .withIsCorner(isCorner)
                    .withCollisionMask(collisionMask)
                    .build();

                raycasts.Add(raycast);
            }

        }
        return raycasts;
    }

    public void AddVelocity(Vector3 amount)
    {
        if (!collisions.tangible) { return; }
        additionalVelocity += amount;
    }

    bool crushTest(bool vertical)
    {
        float rayLength = 4.0f / 32.0f;
        Vector3 dir;
        Vector2 rayOrigin1, rayOrigin2, rayDirection;
        float raySpacing;
        int rayCount;

        if (vertical)
        {
            dir = Vector3.up;

            rayCount = verticalRayCount;

            rayOrigin1 = boundaryPoints.topLeft;
            rayOrigin2 = boundaryPoints.bottomLeft;

            raySpacing = verticalRaySpacing;
            rayDirection = Vector2.right;
        } else
        {
            dir = Vector3.right;

            rayCount = horizontalRayCount;

            rayOrigin1 = boundaryPoints.bottomRight;
            rayOrigin2 = boundaryPoints.bottomLeft;

            raySpacing = horizontalRaySpacing;
            rayDirection = Vector2.up;
        }

        for (int i = 0; i < rayCount; i++)
        {
            RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin1, dir, rayLength, collisionMask);
            RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, -dir, rayLength, collisionMask);

            //if (hit1) {
            //    Debug.DrawLine(rayOrigin1, rayOrigin1 + (Vector2)dir * rayLength, Color.red);
            //} else
            //{
            //    Debug.DrawLine(rayOrigin1, rayOrigin1 + (Vector2)dir * rayLength, Color.green);
            //}
            //if (hit2) {
            //    Debug.DrawLine(rayOrigin2, rayOrigin2 + -(Vector2)dir * rayLength, Color.red);
            //} else
            //{
            //    Debug.DrawLine(rayOrigin2, rayOrigin2 + -(Vector2)dir * rayLength, Color.green);
            //}

            if (hit1 && hit2 && !(hit1.transform.gameObject.Equals(hit2.transform.gameObject)))
            {
                return true;
            }

            rayOrigin1 += rayDirection * (raySpacing);
            rayOrigin2 += rayDirection * (raySpacing);
        }
        return false;
    }

    public void checkWallSlide(float directionX)
    {
        float rayLength = 1 / 32.0f;//skinWidth + 1 / 32.0f;

        int checkMidSection = 0;
        int numberofMidSegments = 2;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? boundaryPoints.bottomLeft : boundaryPoints.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Color c = Color.green;
            if (hit)
            {
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
                c = Color.red;
                if (i < 5)
                {
                    checkMidSection++;
                }
            }
            //Debug.DrawLine(rayOrigin, rayOrigin + Vector2.right * directionX * rayLength, c);
        }
        if (checkMidSection >= numberofMidSegments)
        {
            collisions.wallRideLeft = collisions.left;
            collisions.wallRideRight = collisions.right;
        }
    }

    /* Checks if the collider is grounded and returns the slope of the ground if it is.
     * if not grounded, returns null */
    public Vector2? checkGrounded()
    {
        collisions.isGrounded = false;

        //If moving upwards we aren't grounded
        if (velocity.y > 0) return null;

        /* We cast a bunch of rays down and see if any are within 2 AtlasConstants.StepSizes
         * The reason for 2 is to account for the raycast offset */
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 origin = boundaryPoints.bottomLeft + i * verticalRaySpacing * Vector2.right + Vector2.up * AtlasConstants.StepSize;
            if (i == 0) origin += AtlasConstants.StepSize * Vector2.right;
            if (i == verticalRayCount-1) origin -= AtlasConstants.StepSize * Vector2.right;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, AtlasConstants.StepSize*3, collisionMask);
            if (hit)
            {
                collisions.isGrounded = true;
                return hit.normal;
            }
        }
        return null;
    }

    //I hope we dont need this..
    public bool checkVertDist(float dist)
    {
        float directionY = Mathf.Sign(dist);
        float rayLength = Mathf.Abs(dist);

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == 1.0f) ? boundaryPoints.topLeft : boundaryPoints.bottomLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if (hit)
            {
                rayLength = hit.distance;
                return false;
            }
        }
        return true;
    }

    public bool isSafePosition()
    {
        if (!collisions.tangible) { return false; }
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

    public void UpdateBoundaryPoints()
    {
        CalculateRaySpacing();
        Bounds bounds = collider.bounds;

        boundaryPoints.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        boundaryPoints.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        boundaryPoints.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        boundaryPoints.topRight = new Vector2(bounds.max.x, bounds.max.y);

        //Debug.DrawLine(raycastOrigins.bottomLeft, raycastOrigins.bottomLeft + Vector2.down * 0.25f, Color.blue);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public void setCollidable(bool on = true)
    {
        collisions.collidable = on;
    } 

    struct BoundaryPoints
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;
        public float distanceToGround;
        public bool isGrounded;

        public bool wallRideRight;
        public bool wallRideLeft;

        public bool tangible;
        public bool collidable;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            wallRideLeft = wallRideRight = false;
            climbingSlope = false;
            descendingSlope = false;
            distanceToGround = 50;
            isGrounded = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }

        public void Init()
        {
            Reset();
            tangible = true;
            collidable = true;
        }
    }
}