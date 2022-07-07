using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preventDeform : MonoBehaviour
{
    public bool childrenToo = true;
    public bool revertParentScale = true;
    public bool preventFlip = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        float xscale = 1.0f;
        float yscale = 1.0f;
        float zscale = 1.0f;

        if (revertParentScale && transform.parent != null)
        {
            xscale = 1.0f / transform.parent.localScale.x;
            yscale = 1.0f / transform.parent.localScale.y;
            zscale = 1.0f / transform.parent.localScale.z;
        }
        transform.localScale = new Vector3((preventFlip && transform.localScale.x < 0) ? -xscale : xscale, yscale, zscale);

        if (childrenToo)
        {
            foreach (Transform t in transform)
            {
                t.localScale = new Vector3(t.localScale.x < 0 ? 1 : 1, 1, 1);
            }
        }
    }
}
