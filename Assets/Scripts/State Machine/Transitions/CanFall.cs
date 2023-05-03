using UnityEngine;
using System;

namespace Transitions {
public class CanFall : IStateTransition {
    public override void checkCondition() {
        if (state.controller.velocity.y < -0.1f && 
            !state.controller.isGrounded())
        state.stateMachine.changeState(new States.Fall());
    }
}
}