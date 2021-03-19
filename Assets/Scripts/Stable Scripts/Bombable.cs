using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombable : MonoBehaviour
{
    public GameObject bits;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if  (collision.CompareTag("BombExplosion"))
        {
            if (transform.Find("RibbonLock"))
            {
                transform.Find("RibbonLock").GetComponent<ribbonLockController>().pulse();
                return;
            }
            Instantiate(bits, transform.position, Quaternion.identity);
            if (GetComponent<persistance>() != null) GetComponent<persistance>().MarkRemoved();
            Destroy(gameObject);
        }
    }
}
