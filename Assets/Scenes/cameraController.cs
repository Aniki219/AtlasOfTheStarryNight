using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public Transform target;
    public Collider2D roomBounds;
    Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, -10);
        var h = camera.orthographicSize;
        var w = h * Screen.width / Screen.height;
        float x = Mathf.Clamp(transform.position.x, roomBounds.bounds.min.x + w, roomBounds.bounds.max.x - w);
        float y = Mathf.Clamp(transform.position.y, roomBounds.bounds.min.y + h, roomBounds.bounds.max.y - h);
        transform.position = new Vector3(x, y, -10);
    }
}
