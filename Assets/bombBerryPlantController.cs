using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombBerryPlantController : MonoBehaviour
{
    playerController pc;
    Animator anim;

    void Start()
    {
        pc = gameManager.Instance.playerCtrl;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (pc.state == playerController.State.Bonk)
        {
            Boom();
        }
    }

    void Boom()
    {
        anim.SetTrigger("Boom");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Boom();
        }
    }
}
