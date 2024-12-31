using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class drawColliderEditor : MonoBehaviour
{
    public BoxCollider2D boxCollider { get { return m_boxCollider; } }
    BoxCollider2D m_boxCollider;

    [SerializeField] bool alwaysShowCollider = true;

    void Awake()
    {
        m_boxCollider = GetComponent<BoxCollider2D>();
    }

    void OnDrawGizmos()
    {
        if (alwaysShowCollider)
        {
            Vector2 center = m_boxCollider.bounds.center;
            Vector2 extents = m_boxCollider.bounds.extents;
            Vector2 bottomLeft = center - extents;
            Vector2 topLeft = center - Vector2.Scale(extents, new Vector2(1.0f, -1.0f));
            Vector2 topRight = center + extents;
            Vector2 bottomRight = center + Vector2.Scale(extents, new Vector2(1.0f, -1.0f));
            Gizmos.DrawLine(topLeft, bottomLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(topRight, bottomRight);
        }
    }
}
