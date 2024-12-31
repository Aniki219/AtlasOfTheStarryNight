using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    public float secondsPerRev = 10;
    public Axes axis;
    Vector3 angles;

    Vector3 axesMask;

    public enum Axes
    {
        x = 0,
        y = 1,
        z = 2
    }

    private void Start()
    {
        angles = transform.localEulerAngles;
        axesMask = new Vector3(0, 0, 0);
        axesMask[(int)axis] = 1;
    }

    void Update()
    {
        //angles = transform.localEulerAngles;
        //angles[(int)axis] += -60 * 2 * Mathf.PI * Time.deltaTime / secondsPerRev;
        //transform.rotation = Quaternion.Euler(angles);
        transform.Rotate(axesMask, -60 * 2 * Mathf.PI * Time.deltaTime / secondsPerRev, Space.Self);
    }
}
