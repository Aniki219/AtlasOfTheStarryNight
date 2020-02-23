using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class characterController : MonoBehaviour
{
    public LayerMask collisionMask;
    public LayerMask dangerMask;

    public float safetyMargin = 1f;
    float skinWidth = 0.02f;
    int horizontalRayCount = 6;
    int verticalRayCount = 4;

    float maxClimbAngle = 50;
    float maxDescendAngle = 50;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    new BoxCollider2D collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    Vector3 fixedVelocity;
    Vector3 additionalVelocity;

    public Vector3 cameraTarget;

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collisions.tangible = true;
        CalculateRaySpacing();
    }

    public void AddVelocity(Vector3 amount)
    {
        if (!collisions.tangible) { return;  }
        additionalVelocity += amount;
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
        transform.Translate(additionalVelocity);

        cameraTarget = velocity/Time.deltaTime;

        if ((additionalVelocity.y != 0 && crushTest(true)) || (additionalVelocity.x != 0 && crushTest(false)))
        {
            playerController pc = GetComponent<playerController>();
            if (pc)
            {
                pc.startBonk(1, true);
            }
        }

        crushTest(true);

        additionalVelocity = Vector3.zero;
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

            rayOrigin1 = raycastOrigins.topLeft;
            rayOrigin2 = raycastOrigins.bottomLeft;

            raySpacing = verticalRaySpacing;
            rayDirection = Vector2.right;
        } else
        {
            dir = Vector3.right;

            rayCount = horizontalRayCount;

            rayOrigin1 = raycastOrigins.bottomRight;
            rayOrigin2 = raycastOrigins.bottomLeft;

            raySpacing = horizontalRaySpacing;
            rayDirection = Vector2.up;
        }

        for (int i = 0; i < rayCount; i++)
        {
            RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin1, dir, rayLength, collisionMask);
            RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, -dir, rayLength, collisionMask);

            if (hit1) {
                Debug.DrawLine(rayOrigin1, rayOrigin1 + (Vector2)dir * rayLength, Color.red);
            } else
            {
                Debug.DrawLine(rayOrigin1, rayOrigin1 + (Vector2)dir * rayLength, Color.green);
            }
            if (hit2) {
                Debug.DrawLine(rayOrigin2, rayOrigin2 + -(Vector2)dir * rayLength, Color.red);
            } else
            {
                Debug.DrawLine(rayOrigin2, rayOrigin2 + -(Vector2)dir * rayLength, Color.green);
            }

            if (hit1 && hit2 && !(hit1.transform.gameObject.Equals(hit2.transform.gameObject)))
            {
                return true;
            }

            rayOrigin1 += rayDirection * (raySpacing);
            rayOrigin2 += rayDirection * (raySpacing);
        }
        return false;
    }

    //void FixedUpdate()
    //{
    //    transform.Translate(fixedVelocity);
    //    fixedVelocity = Vector3.zero;
    //}

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth + 1/32f;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;   
                rayLength = hit.distance;
                if (directionY == -1)
                {
                    collisions.distanceToGround = Mathf.Min(collisions.distanceToGround, hit.distance);
                }

                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }

            //Debug.DrawLine(rayOrigin, rayOrigin + Vector2.up * directionY * rayLength);
        }

        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);

            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    public bool isSafePosition()
    {
        if (!collisions.tangible) { return false; }
        Vector3 boxCastOrigin = collider.transform.position - Vector3.right * safetyMargin/2;
        RaycastHit2D dhit = Physics2D.BoxCast(boxCastOrigin, collider.size, 0, Vector3.right, safetyMargin, dangerMask);
        if (dhit)
        {
            return false;
        }
        int rays = 0;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomLeft + (Vector2.right * verticalRaySpacing * i);
            float maxDistance = 1 / 32f + skinWidth;

            LayerMask safetyMask = LayerMask.NameToLayer("Wall");
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, maxDistance, 1 << safetyMask);

            if (hit)
            {
                rays++;
            }
        }
        return rays > 1;
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
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

        public bool wallRideRight;
        public bool wallRideLeft;

        public bool tangible;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            distanceToGround = 50;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}