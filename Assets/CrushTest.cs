using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrushTest : MonoBehaviour
{
    Collider2D col;

    public UnityEvent callback;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    public void test(float vy)
    {
        float rayLength = Mathf.Abs(vy);
        float verticalRaySpacing = transform.localScale.x * col.bounds.size.x / 4.0f;
        Vector2 rayOrigin = Vector3.Scale(col.bounds.center -
                col.bounds.extents.x * Vector3.right +
                col.bounds.extents.y * Vector3.up,
                transform.localScale);

        for (int i = 0; i < 4; i++)
        {
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            Debug.DrawRay(rayOrigin, Vector3.up*vy, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, 1 << LayerMask.NameToLayer("Wall"));

            if (hit)
            {
                Debug.Log("crush!");
                callback.Invoke();
                return;
            }
        }
    }
}
