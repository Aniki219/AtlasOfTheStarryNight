using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    public float secondsPerRev = 10;
    public Axes axes;
    Vector3 angles;

    public enum Axes
    {
        x,y,z
    }

    private void Start()
    {
        angles = transform.localEulerAngles;
    }

    void Update()
    {
        angles[(int)axes] += -60 * 2 * Mathf.PI * Time.deltaTime / secondsPerRev;
        transform.rotation = Quaternion.Euler(angles);
    }
}
