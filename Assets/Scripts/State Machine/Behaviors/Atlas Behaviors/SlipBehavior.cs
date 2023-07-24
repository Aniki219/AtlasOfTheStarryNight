using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Transitions;

namespace Behaviors {
[Serializable]
public class SlipBehavior : IStateBehavior
{
    PlayerController pc;
    float startTime;
    float timeToSpinJump = 0.1f;

    public async override Task StartBehavior() {
        pc = (PlayerController)state.stateMachine;
        pc.setFacing(AtlasHelpers.Sign(state.controller.collisions.getGroundSlope().x));

        state.controller.velocity.x = 0;

        startTime = Time.time;
        
        await Task.Yield();
    }

    public override void UpdateBehavior() {
        if (state.controller.collisions.getGroundSlope().y > state.controller.maxSlope) {
            pc.changeState(new States.Idle());
        }
        if (Time.time - startTime > timeToSpinJump) {
            state.GetTransition<CanJump<States.GroundJump>>().Pause();
        }
    }

    public async override Task ExitBehavior() {
        await Task.Yield();
    }
}
}
