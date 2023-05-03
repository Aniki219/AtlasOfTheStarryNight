using UnityEngine;
using System;

namespace Transitions {
    public class CanLand : IStateTransition {
        bool attacking;

        public CanLand(bool attacking = false) {
            this.attacking = attacking;
        }
        public override void checkCondition() {
            if (state.controller.isGrounded() && state.controller.velocity.y <= 0) {
                if (attacking) {
                    changeState(new States.AttackLand());
                } else {
                    changeState(new States.Land());
                }
            }
        }
    }
}