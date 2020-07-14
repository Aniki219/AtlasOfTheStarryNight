using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class translator : MonoBehaviour
{
    public float speed = 2.0f;
    public Vector2 direction = Vector2.right;

    void Update()
    {
        transform.Translate(speed * direction * Time.deltaTime);
    }
}
