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
//        stateName = transform.Find("EnemyCanvas/StateName").GetComponent<TextMeshProUGUI>();
        animClipPrefix = "diveToad";
        base.Start();
    }

    protected override void changePhase(Phase to) {
        base.changePhase(to);
  //      stateName.text = state.GetType().Name + ": " + to;
    }

    public override void hurt(HitBox hitbox) {
        ChangeState(new Hurt().HitBox(hitbox));
    }
}
