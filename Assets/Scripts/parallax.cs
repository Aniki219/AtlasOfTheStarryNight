using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    [Range(0.01f, 1f)] public float parallaxScale = 1f;
    private Transform cam;
    Vector3 startPosition;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    private void Start()
    {
        startPosition = cam.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 displacement = cam.transform.position - startPosition;

        foreach (Transform child in transform)
        {
            float z = child.transform.position.z;
            child.transform.position += (displacement.x * z / (z + 1/parallaxScale) ) * Vector3.right;
        }

        startPosition = cam.transform.position;
    }
}
