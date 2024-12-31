using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Behaviors {
[Serializable]
public class NovaBehavior : IStateBehavior
{
    PlayerController pc;
    NovaManager nm;
    NovaManager.GainSpeed gainSpeed = NovaManager.GainSpeed.Clear;

    public NovaBehavior GainSpeed(NovaManager.GainSpeed _gainSpeed) {
        gainSpeed = _gainSpeed;
        return this;
    }

    public async override Task StartBehavior() {
        pc = (PlayerController)state.stateMachine;
        nm = pc.GetComponent<NovaManager>();
        nm.setGain(gainSpeed);
        await Task.Yield();
    }

    public override void UpdateBehavior() {
        if (nm.isNovaDashing()) {
            pc.broomEffects.SetNovaSparkles(true);
        }
    }

    public async override Task ExitBehavior() {
        await Task.Yield();
    }

    public override void PostExitBehavior() {
        nm.ResetRequest();
        pc.broomEffects.SetNovaSparkles(false);
        nm.setNovaDashing(false);
    }
}
}
