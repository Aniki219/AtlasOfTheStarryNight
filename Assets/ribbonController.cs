using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ribbonController : MonoBehaviour
{
    public Transform backRibbon;
    public bool collected = false;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        AtlasEventManager.Instance.onPlayerLand += OnLanding;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collected && collision.CompareTag("Player") &&
            gameManager.Instance.playerCtrl.state == playerController.State.Broom)
        {
            collected = true;
            anim.SetTrigger("FlyAway");
            backRibbon.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void OnLanding()
    {
        if (!collected) return;
        GameObject[] ribbons = GameObject.FindGameObjectsWithTag("Ribbon");
        foreach (GameObject r in ribbons)
        {
            if (!r.GetComponent<ribbonController>().collected)
            {
                resetRibbon();
                return;
            }
        }
        Destroy(gameObject);
    }

    void resetRibbon()
    {
        collected = false;
        anim.SetTrigger("Reset");
        backRibbon.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void OnDestroy()
    {
        AtlasEventManager.Instance.onPlayerLand -= OnLanding;
    }
}
