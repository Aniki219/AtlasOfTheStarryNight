using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windmillController : MonoBehaviour
{
    public Transform player;
    public float radius = 1;
    Vector3 startPos;
    float startAngle;
    public Transform tower;

    private void Start()
    {
        startPos = transform.position;
        startAngle = transform.localEulerAngles.y;
    }
    
    void Update()
    {
        transform.position = new Vector3(player.position.x, startPos.y, startPos.z);
        float dx = player.position.x - startPos.x;
        float theta = startAngle + dx / radius;
        tower.localRotation = Quaternion.Euler(Vector3.right * -90 + Vector3.up * theta);
    }
}
