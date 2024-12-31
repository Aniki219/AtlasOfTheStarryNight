using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windmillController : MonoBehaviour
{
    public Transform player;
    public float radius = 1;
    Vector3 startPos;
    float startAngle;

    private void Start()
    {
        startPos = transform.position;
        startAngle = transform.localEulerAngles.y;
    }
    
    void FixedUpdate()
    {
        transform.position = new Vector3(player.position.x, startPos.y, startPos.z);
        float s = player.position.x - startPos.x;
        float radius = transform.position.z;
        float theta = s / radius;
        theta *= Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(Vector3.right * -90 + Vector3.up * theta);
    }
}
