using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preventParentRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, transform.root.rotation.z * -1.0f);
    }
}
