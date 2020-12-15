using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyHitBoxController : MonoBehaviour
{
    public HitBox hitbox;

    //If you want to preventing hitting multiple things at once
    //have that object check if this is false and then set it to true when hit.
    public bool hasHit = false;

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
