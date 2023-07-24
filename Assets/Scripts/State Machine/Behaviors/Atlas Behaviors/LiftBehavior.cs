using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Behaviors {
[Serializable]
public class LiftBehavior : IStateBehavior
{
    PlayerController pc;
    Transform obj;
    public async override Task StartBehavior() {
        pc = (PlayerController)state.stateMachine;
        obj = pc.liftableObject;

        liftController lc = obj.GetComponent<liftController>();
        Rigidbody2D rb = obj.GetComponentInParent<Rigidbody2D>();

        if (lc && rb)
        {
            pc.liftableObject = null;
            float liftTime = 0.22f;
            pc.setFacing(Mathf.Sign(obj.transform.position.x - state.transform.position.x));
            rb.velocity = Vector2.zero;
            rb.simulated = false;
            lc.startLift(new Vector3(0.4f, 0.4f, 0), new Vector3(0, 0.4f, 0), state.transform, liftTime);
            pc.resetAnimator();
            pc.resetVelocity();
            //pc.freezeForSeconds(liftTime + 0.15f, true);
            await AtlasHelpers.WaitSeconds(liftTime);
            pc.heldObject = obj.transform.parent;
        }
        //TODO: No literals in delay
        await AtlasHelpers.WaitSeconds(0.15f);
        pc.changeState(new States.Carry());
    }

    public override void UpdateBehavior() {
        
    }

    public async override Task ExitBehavior() {
        await Task.Yield();
    }
}

}