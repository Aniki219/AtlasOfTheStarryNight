using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treeCollisionBox : MonoBehaviour
{
    BoxCollider2D[] cols;
    GameObject player;
    oscillator os;
    // Start is called before the first frame update
    void Start()
    {
        cols = GetComponents<BoxCollider2D>();
        os = GetComponent<oscillator>();
        player = GameObject.Find("Atlas");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y >= transform.position.y - 0.25f)
        {
            foreach (BoxCollider2D col in cols) {
                col.enabled = false;
            }
        }
        else if (Mathf.Abs(player.transform.position.x - transform.position.x) > 0.6f)
        {
            foreach (BoxCollider2D col in cols)
            {
                col.enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerController pc = collision.GetComponent<playerController>();
            if (pc.depState == playerController.State.Broom)
            {
                StartCoroutine(shake());
                foreach (Transform t in transform)
                {
                    t.GetComponent<fruitController>().fall();
                }
            }
        }
    }

    IEnumerator shake()
    {
        os.enabled = true;
        yield return new WaitForSeconds(.5f);
        os.enabled = false;
    }
}
