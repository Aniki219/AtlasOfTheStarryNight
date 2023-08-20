using UnityEngine;
using System;
using States.DiveToad;

//TODO: This is how we generalize states. 
// Anyone can use this
namespace Transitions.DiveToad {
    public class CanFall<T> : IStateTransition where T : State {
        public override void checkCondition() {
            if (state.controller.velocity.y < -0.01f && 
                !state.controller.isGrounded()) {
                changeState<T>();
            }
        }
    }
}
