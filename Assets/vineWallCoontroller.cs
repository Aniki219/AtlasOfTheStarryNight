using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vineWallCoontroller : MonoBehaviour
{
    Animator anim;
    Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void reset()
    {
        anim.SetBool("isUp", false);
        col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!anim.GetBool("isUp") && collision.CompareTag("AllyHitbox"))
        {
            anim.SetBool("isUp", true);
            col.enabled = false;
            Invoke("reset", 3.5f);
        }
    }
}
