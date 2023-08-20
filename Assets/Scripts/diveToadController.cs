using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States.DiveToad;

using TMPro;

public class DiveToadController : EnemyController
{
    public bool readyToPounce = false;
    TextMeshProUGUI stateName;
    
    public override void Start() {
        startState = new Idle();
        stateName = GetComponent<TextMeshProUGUI>();
        animClipPrefix = "diveToad";
        base.Start();
    }

    protected override void changePhase(Phase to) {
        base.changePhase(to);
        transform.Find("EnemyCanvas/StateName").GetComponent<TextMeshProUGUI>().text = state.GetType().Name + ": " + to;
    }

    public void hurt(HitBox hitbox)
    {
        float kbStrength = (hitbox.knockback ? 2.5f : 1.5f);
        float dx = hitbox.kbDir.x;
        float dy = hitbox.kbDir.y;

        if (hitbox.explosive)
        {
            Vector2 dir = (transform.position - hitbox.position).normalized;
            dx = dir.x;
            dy = dir.y;
        }
        controller.velocity.x = kbStrength * dx;
        controller.velocity.y = kbStrength * 1.5f * dy;
        act = Mathf.Max(actionCoolDown / 2.0f, act);
        anim.SetBool("Hurt", true);

        //StartCoroutine(getHurt());
    }

    // private void FixedUpdate()
    // {
    //     controller.Move(controller.velocity * Time.deltaTime);
    // }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.tag == "AllyHitbox")
    //    {
    //        HitBox hb = other.GetComponent<AllyHitBoxController>().hitbox;
    //        hurt(hb, false);
    //    }
    //}
}
