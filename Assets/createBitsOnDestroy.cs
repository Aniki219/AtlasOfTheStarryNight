using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createBitsOnDestroy : MonoBehaviour
{
    public GameObject bits;

    public void destroySelf()
    {
        Destroy(gameObject,  0.1f);
    }

    private void OnDestroy()
    {
        gameManager.createInstance(bits, transform.position);
    }
}
