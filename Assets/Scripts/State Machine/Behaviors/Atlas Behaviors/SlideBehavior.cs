using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Behaviors {
[Serializable]
public class SlideBehavior : IStateBehavior
{
    public async override Task StartBehavior() {
        await Task.Yield();
    }

    public override void UpdateBehavior() {
        
    }

    public async override Task ExitBehavior()
    {
        await Task.Yield();
    }

    private async void slide() {
        state.anim.SetTrigger("slide");
        state.controller.velocity.x = state.deformer.getFacing() * 6;
        await Task.Yield();
    }
}

}