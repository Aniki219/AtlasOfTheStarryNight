using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public Transform target;
    public Collider2D roomBounds;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, -10);
        float h = cam.scaledPixelHeight/128.0f;
        float w = cam.scaledPixelWidth/128.0f;

        float x = Mathf.Clamp(transform.position.x, roomBounds.bounds.min.x + w, roomBounds.bounds.max.x - w);
        float y = Mathf.Clamp(transform.position.y, roomBounds.bounds.min.y + h, roomBounds.bounds.max.y - h);
        transform.position = new Vector3(x, y, -10);
    }
}
