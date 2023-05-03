using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Behaviors {
[Serializable]
public class LandBehavior : IStateBehavior
{
    private int duration;

    public LandBehavior(int duration = 0) {
        this.duration = duration;
    }

    public async override Task StartBehavior() {
        if (duration > 0) {
            state.anim.SetTrigger("attackLand");
            state.particleMaker.createDust(true);
            state.controller.velocity = Vector3.zero;
        }
        await Task.Delay(duration);
        state.stateMachine.changeState(new States.Move());
    }

    public override void UpdateBehavior() {
        
    }

    public async override Task ExitBehavior() {
        await Task.Yield();
    }
}
}
