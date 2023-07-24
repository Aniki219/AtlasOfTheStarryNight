using UnityEngine;
using System;

namespace Transitions {
public class CanBroom<T> : IStateTransition where T : States.Broom {
    PlayerController pc;

    public override void attach(State state) {
        base.attach(state);
        pc = (PlayerController)state.stateMachine;
    }

    public override void checkCondition() {
        if (Input.GetButtonDown("Broom")) {
            if (pc.resetPosition || !state.controller.collisions.isTangible()) return;
            //Cancel is ceiling above while crouching
            if (pc.checkState(new States.Crouch()) && !state.controller.checkVertDist(0.3f)) return;
            States.Broom broomStateObj = (T) Activator.CreateInstance(typeof(T));
            if (broomStateObj.requiresNova && !pc.novaManager.isCharged()) return;
            pc.changeState(broomStateObj);
        }
    }
}

public class CanBroom : CanBroom<States.Broom> {}
}