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
        transform.localScale = new Vector3(transform.localScale.x < 0 ? -1 : 1, 1, 1);
        foreach (Transform t in transform)
        {
            t.localScale = new Vector3(t.localScale.x < 0 ? 1 : 1, 1, 1);
        }
    }
}
