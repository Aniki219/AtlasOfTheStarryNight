using UnityEngine;
using System;

namespace Transitions {
    public class CanLand : IStateTransition {
        bool attacking;

        public override void checkCondition() {
            if (state.controller.isGrounded() && state.controller.velocity.y <= 0) {
                doLand();
            }
        }

        public virtual void doLand() {
            changeState(new States.Land());
        }
    }

    public class CanLand<T> : CanLand where T : States.Land {
        bool attacking;

        public override void doLand() {
            changeState(getNewState<T>());
        }
    }
}