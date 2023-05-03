using UnityEngine;
using System;

namespace Transitions {
    public class CanSlip : IStateTransition {
        playerController pc;
        characterController cc;

        public override void attach(State state) {
            base.attach(state);
            pc = (playerController)state.stateMachine;
            cc = state.controller;
        }

        public override void checkCondition() {
            if (pc.isGrounded() && 
                    cc.collisions.getGroundSlope().y < 0.5f && 
                    cc.velocity.y < -0.5f) {
                changeState(new States.Slip());
            }
        }
    }
}