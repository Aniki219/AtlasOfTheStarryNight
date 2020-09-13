﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oscillator : MonoBehaviour
{
    public float cyclesPerSecond = 1.0f;
    public Axes axis;
    Vector3 startPos;
    Vector3 oscillationDirection;
    public float oscillationSize = 5.0f;
    public bool rotational = false;
    Vector3 startPosition;

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
        if (rotational)
        {
            transform.localEulerAngles -= oscillationDirection * oscillationSize * 0.5f;
        }
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    { 
        frameCount += Time.deltaTime;
        if (rotational)
        {
            float angle = Mathf.Sin(frameCount * 2.0f * Mathf.PI * cyclesPerSecond);
            transform.localRotation = Quaternion.Euler(oscillationDirection * angle * oscillationSize);
        } else
        {
            transform.position = startPosition + oscillationDirection * Mathf.Sin(frameCount * 2.0f * Mathf.PI * cyclesPerSecond) * oscillationSize;
        }
    }
}
