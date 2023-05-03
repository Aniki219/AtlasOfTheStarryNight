using UnityEngine;
using System;

namespace Transitions {
public class CanBroom : IStateTransition {
    playerController pc;

    public override void attach(State state) {
        base.attach(state);
        pc = (playerController)state.stateMachine;
    }

    public override void checkCondition() {
        if (Input.GetButtonDown("Broom")) {
            if (pc.resetPosition || !state.controller.collisions.isTangible()) return;
            //Cancel is ceiling above while crouching
            if (pc.checkState(new States.Crouch()) && !state.controller.checkVertDist(0.3f)) return;
            pc.changeState(new States.Broom());
        }
    }
}
}