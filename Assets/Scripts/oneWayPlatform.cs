using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneWayPlatform : MonoBehaviour
{
    BoxCollider2D col;
    GameObject player;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Atlas");
    }

    // Update is called once per frame
    void Update()
    {
        if (!player) { return; }
        if (player.transform.position.y > transform.position.y + player.GetComponent<BoxCollider2D>().size.y)
        {
            col.enabled = true;
        } else if (player.transform.position.y < transform.position.y + 10/32f)
        {
            col.enabled = false;
        }

        //if (col.enabled)
        //{
        //    GetComponent<SpriteRenderer>().color = Color.green;
        //}
        //else
        //{
        //    GetComponent<SpriteRenderer>().color = Color.red;
        //}
    }
}
