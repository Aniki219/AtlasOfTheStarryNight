using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombable : MonoBehaviour
{
    public GameObject bits;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if  (collision.CompareTag("BombExplosion"))
        {
            Instantiate(bits, transform.position, Quaternion.identity);
            if (GetComponent<persistance>() != null) GetComponent<persistance>().MarkRemoved();
            Destroy(gameObject);
        }
    }
}
