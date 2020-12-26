using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preventDeform : MonoBehaviour
{
    public bool childrenToo = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.localScale = Vector3.one;
        foreach (Transform t in transform)
        {
            t.localScale = Vector3.one;
        }
    }
}
