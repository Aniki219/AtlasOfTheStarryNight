using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AtlasRaycast : IComparable<AtlasRaycast>
{
    public Vector2 castVector { get; private set; }

    public readonly Vector2 origin;
    public readonly Vector2 originOffset;
    public readonly bool isCorner;
    public readonly int collisionMask;
    public readonly Vector2 direction;

    public Vector2 incident { get; private set; }
    public Vector2 penetrant { get; private set; }
    public Vector2 surfaceNormal { get; private set; }
    public Vector2 adjustment { get; private set; }
    public Vector2 floatVector { get; private set; }

    public enum Slide {
        ALL,
        NOT_DOWN,
        NONE
    }

    private AtlasRaycast(AtlasRaycastBuilder builder)
    {
        castVector = builder.castVector;

        origin = builder.origin;
        originOffset = builder.originOffset;
        isCorner = builder.isCorner;
        collisionMask = builder.collisionMask;
        direction = builder.castVector.normalized;

        incident = Vector2.zero;
        penetrant = Vector2.zero;
        surfaceNormal = Vector2.zero;
        adjustment = Vector2.zero;
        floatVector = Vector2.zero;
    }

    public void cast(Slide slide = Slide.NOT_DOWN)
    {
        Vector2 desiredDirection = castVector.normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, castVector.normalized, castVector.magnitude, collisionMask);

        if (hit.collider != null)
        {
            incident = hit.distance * castVector.normalized;
            penetrant = castVector - incident;
            surfaceNormal = hit.normal;
        } else
        {
            incident = castVector;
            return;
        }

        /* We calculate the adjustment vector only if we can slide
             * The float vector is a slight offset in the direction of the surface normal
             * whose magnitude places the collider box of the player exactly on the
             * surface.
             * If !canSlide we want to ignore the surface normal and just move backwards
             * */
        floatVector = Vector2.Dot(originOffset, surfaceNormal) * surfaceNormal;
        if (slide != Slide.ALL)
        {
            penetrant = new Vector2(penetrant.x, Mathf.Max(penetrant.y, 0));

            if (Mathf.Abs(desiredDirection.x) < AtlasConstants.StepSize && desiredDirection.y < 0)
            {
                floatVector = Vector2.Dot(originOffset, -desiredDirection) * -desiredDirection;
            }
        }

        adjustment = Vector2.Dot(-penetrant, surfaceNormal) * surfaceNormal;
    }

    public Vector2 calcResultant()
    {
        return incident + penetrant + adjustment + floatVector;
    }

    public void drawVectors()
    {
        Debug.DrawLine(origin, origin + incident, Color.red);
        Debug.DrawLine(origin + incident, origin + incident + penetrant, Color.blue);
        Debug.DrawLine(origin + incident + penetrant, origin + incident + penetrant + adjustment, Color.magenta);
        Debug.DrawLine(origin + incident + penetrant + adjustment, origin + incident + penetrant + adjustment + floatVector, Color.green);
        Debug.DrawLine(origin, origin + calcResultant(), Color.white);
    }

    public void setCastVectorLength(float length)
    {
        castVector = castVector.normalized * length;
    }

    public static AtlasRaycastBuilder builder() { return new AtlasRaycastBuilder(); }

    public int CompareTo(AtlasRaycast other)
    {
        if (other == null) return 1;
        else return incident.magnitude.CompareTo(other.incident.magnitude);
    }

    public class AtlasRaycastBuilder
    {
        public Vector2 origin;
        public Vector2 originOffset;
        public Vector2 castVector;
        public bool isCorner;
        public int collisionMask;

        public AtlasRaycastBuilder() {

        }

        public AtlasRaycastBuilder withOrigin(Vector2 originToUse)
        {
            origin = originToUse;
            return this;
        }

        public AtlasRaycastBuilder withOriginOffset(Vector2 originOffsetToUse)
        {
            originOffset = originOffsetToUse;
            return this;
        }

        public AtlasRaycastBuilder withCastVector(Vector2 castVectorToUse)
        {
            castVector = castVectorToUse;
            return this;
        }

        public AtlasRaycastBuilder withIsCorner(bool isCornerToUse)
        {
            isCorner = isCornerToUse;
            return this;
        }

        public AtlasRaycastBuilder withCollisionMask(int collisionMaskToUse)
        {
            collisionMask = collisionMaskToUse;
            return this;
        }

        public AtlasRaycast build()
        {
            origin += originOffset;
            return new AtlasRaycast(this);
        }
    }
}
