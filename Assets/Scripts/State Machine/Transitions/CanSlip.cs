using UnityEngine;
using System;

namespace Transitions {
    public class CanSlip : IStateTransition {
        PlayerController pc;
        characterController cc;

        public override void attach(State state) {
            base.attach(state);
            pc = (PlayerController)state.stateMachine;
            cc = state.controller;
        }

        public override void checkCondition() {
            if (pc.isGrounded() && 
                    cc.collisions.getGroundSlope().y < 0.5f && 
                    cc.collisions.getGroundSlope().y > 0.1f && 
                    cc.velocity.y < -0.1f) {
                changeState(new States.Slip());
            }
        }
    }
}