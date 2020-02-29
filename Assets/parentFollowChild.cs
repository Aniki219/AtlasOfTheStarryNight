using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parentFollowChild : MonoBehaviour
{
    public GameObject character;
    void FixedUpdate()
    {
        transform.position = character.transform.position;
    }
}
