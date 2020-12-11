using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyHitBoxController : MonoBehaviour
{
    public HitBox hitbox;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (hitbox.bounce)
            {
                gameManager.Instance.playerCtrl.bounce(5.5f);
            }
        }
    }
}
