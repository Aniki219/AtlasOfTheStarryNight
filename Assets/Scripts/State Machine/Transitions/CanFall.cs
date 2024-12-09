using UnityEngine;
using System;

namespace Transitions {
public class CanFall : IStateTransition {
    public override void checkCondition() {
        if (state.controller.velocity.y < -0.01f && 
            !state.controller.IsGrounded())
        state.stateMachine.ChangeState(new States.Fall());
    }
}
}