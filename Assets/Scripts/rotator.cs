using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    public float secondsPerRev = 10;
    float angle = 0f;

    void Update()
    {
        angle += -60 * 2 * Mathf.PI * Time.deltaTime / secondsPerRev;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, angle);
    }
}
