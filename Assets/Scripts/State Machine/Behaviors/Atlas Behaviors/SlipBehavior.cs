using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Behaviors {
[Serializable]
public class SlipBehavior : IStateBehavior
{
    playerController pc;

    public async override Task StartBehavior() {
        pc = (playerController)state.stateMachine;
        pc.setFacing(AtlasHelpers.Sign(state.controller.collisions.getGroundSlope().x));
        
        state.anim.SetBool("isSlipping", true);
        state.controller.velocity.x = 0;
        
        await Task.Yield();
    }

    public override void UpdateBehavior() {
        if (state.controller.collisions.getGroundSlope().y > state.controller.maxSlope) {
            pc.changeState(new States.Idle());
        }
    }

    public async override Task ExitBehavior() {
        state.anim.SetBool("isSlipping", false);
        await Task.Yield();
    }
}
}
