using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombable : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if  (collision.CompareTag("BombExplosion"))
        {
            Destroy(gameObject);
        }
    }
}
