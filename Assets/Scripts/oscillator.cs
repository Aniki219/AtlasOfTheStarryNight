using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oscillator : MonoBehaviour
{
    public float cyclesPerSecond = 1.0f;
    public Axes axis;
    Vector3 oscillationDirection;
    public float oscillationSize = 5.0f;
    public bool rotational = false;
    Vector3 startPosition;
    Vector3 startRotation;

    float frameCount = 0.0f;

    [SerializeField]
    public List<Oscillator> oscillators;

    public enum Axes
    {
        x = 0,
        y = 1,
        z = 2
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogWarning("Phasing out oscillator. Use Deformer instead. (" + gameObject.name + ")");
        //frameCount is to remain sync'd for all oscillators
        frameCount = (int)Random.Range(0, 100) * Time.deltaTime;
        oscillationDirection = Vector3.zero;

        switch ((int)axis)
        {
            case 0:
                oscillationDirection = transform.right;
                break;
            case 1:
                oscillationDirection = transform.up;
                break;
            case 2:
                oscillationDirection = transform.forward;
                break;
        }
        if (rotational)
        {
            transform.localEulerAngles -= oscillationDirection * oscillationSize * 0.5f;
        }
        startPosition = transform.localPosition;
        startRotation = transform.localEulerAngles;

        //Setup Oscillators
        foreach (Oscillator o in oscillators)
        {
            switch ((int)o.axis)
            {
                case 0:
                    o.oscillationDirection = transform.right;
                    break;
                case 1:
                    o.oscillationDirection = transform.up;
                    break;
                case 2:
                    o.oscillationDirection = transform.forward;
                    break;
            }
            if (o.rotational)
            {
                transform.localEulerAngles -= o.oscillationDirection * o.oscillationSize * 0.5f;
            }
        }
    }

    // Update is called once per frame
    void Update()
    { 
        frameCount += Time.deltaTime;
        if (rotational)
        {
            float angle = Mathf.Sin(frameCount * 2.0f * Mathf.PI * cyclesPerSecond);
            transform.localRotation = Quaternion.Euler(startRotation + oscillationDirection * angle * oscillationSize * transform.localScale.y); //Upside down switches cw to ccw i think..
        } else
        {
            transform.position = startPosition + oscillationDirection * Mathf.Sin(frameCount * 2.0f * Mathf.PI * cyclesPerSecond) * oscillationSize;
        }


        //Update Oscillators
        foreach (Oscillator o in oscillators)
        {
            if (!o.enabled) continue;
            if (o.rotational)
            {
                float angle = Mathf.Sin(frameCount * 2.0f * Mathf.PI * o.cyclesPerSecond);
                transform.localRotation = Quaternion.Euler(
                    startRotation + 
                    o.oscillationDirection * 
                    angle * 
                    o.oscillationSize * 
                    transform.localScale.y); //Upside down switches CW to CCW
            }
            else
            {
                transform.position = startPosition + o.oscillationDirection * Mathf.Sin(frameCount * 2.0f * Mathf.PI * o.cyclesPerSecond) * o.oscillationSize;
            }
        }
    }

    public void SetOscillatorActive(string tag = "default", bool on = true)
    {
        foreach (Oscillator o in oscillators)
        {
            if (o.tag == tag)
            {
                o.enabled = on;
            }
        }
    }
}
