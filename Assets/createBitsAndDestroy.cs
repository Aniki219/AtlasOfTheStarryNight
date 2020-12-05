using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createBitsAndDestroy : MonoBehaviour
{
    public GameObject bits;

    public void createBits()
    {
        gameManager.createInstance(bits, transform.position);
    }
}
