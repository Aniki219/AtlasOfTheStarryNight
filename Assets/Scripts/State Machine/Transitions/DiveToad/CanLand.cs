using UnityEngine;
using System;
using States.DiveToad;

namespace Transitions.DiveToad {
    
    public class CanLand : IStateTransition {
        public override void checkCondition() {
            if (state.controller.isGrounded() && state.controller.velocity.y <= 0) {
                changeState(new Land());
            }
        }
    }
}
