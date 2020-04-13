using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraParallax : MonoBehaviour
{
    cameraController cc;
    Collider2D roomBounds;
    Collider2D col;

    float minX, minY;
    float maxX, maxY;

    // Start is called before the first frame update
    void Start()
    {
        cc = Camera.main.GetComponent<cameraController>();
        roomBounds = cc.roomBounds;
        col = GetComponent<BoxCollider2D>();

        minX = roomBounds.bounds.center.x - roomBounds.bounds.extents.x + col.bounds.extents.x;
        maxX = roomBounds.bounds.center.x + roomBounds.bounds.extents.x - col.bounds.extents.x;

        minY = roomBounds.bounds.center.y - roomBounds.bounds.extents.y + col.bounds.extents.y;
        maxY = roomBounds.bounds.center.y + roomBounds.bounds.extents.y - col.bounds.extents.y;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cc.percentX * (maxX - minX) + minX, transform.position.y, transform.position.z);
        transform.position = new Vector3(transform.position.x, cc.percentY * (maxY - minY) + minY, transform.position.z);
    }
}
