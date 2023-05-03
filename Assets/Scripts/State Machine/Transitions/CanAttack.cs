using UnityEngine;
using System;

namespace Transitions {
public class CanAttack : IStateTransition {
    public override void checkCondition() {
        if (AtlasInputManager.getKeyPressed("Attack")) {
            if (state.controller.isGrounded()) {
                changeState(new States.Attack());
            } else {
                if (state.controller.velocity.y >= 0) {
                    changeState(new States.AirialAttacks.RisingFair());
                } else {
                    changeState(new States.AirialAttacks.FallingFair());
                }
            }
        }
    }
}
}