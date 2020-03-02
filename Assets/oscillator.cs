using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oscillator : MonoBehaviour
{
    public float cyclesPerSecond = 1.0f;
    public Axes axis;
    Vector3 startPos;
    Vector3 oscillationDirection;
    public float oscillationSize = 5.0f;

    float frameCount = 0.0f;

    public enum Axes
    {
        x = 0,
        y = 1,
        z = 2
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        oscillationDirection = Vector3.zero;
        oscillationDirection[(int)axis] = 1;
    }

    // Update is called once per frame
    void Update()
    {
        frameCount += 1.0f;
        transform.position += oscillationDirection * Mathf.Sin(frameCount * 2.0f * Mathf.PI / 60.0f * cyclesPerSecond) * oscillationSize/32.0f;
    }
}
