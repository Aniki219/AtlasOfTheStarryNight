using UnityEngine;
using System;

namespace Transitions {
    public class CanLift : IStateTransition {
        PlayerController pc;

        public override void attach(State state) {
            base.attach(state);
            pc = (PlayerController)state.stateMachine;
        }
        public override void checkCondition() {
            // if (Input.GetButtonDown("Up")) {
            //     if (!state.controller.isGrounded()) return;
            //     if (pc.liftableObject) {
            //         pc.changeState(new States.Lift);
            //     }
            // }
        }
    }
}