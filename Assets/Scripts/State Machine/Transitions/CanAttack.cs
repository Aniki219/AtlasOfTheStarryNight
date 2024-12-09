using UnityEngine;
using System;
using States;
using States.AerialAttacks;
using States.Attacks;

namespace Transitions {
public class CanAttack : IStateTransition {
    public override void checkCondition() {
        if (AtlasInputManager.getKeyPressed("Attack")) {
            bool arial = !state.controller.IsGrounded();
            bool falling = state.controller.velocity.y < 0;
            TiltInfo direction = AtlasInputManager.getAxis("Dpad").getDirection();

            if (arial) {
                if (direction.y.Equals(TiltDirection.Up)) {
                    changeState(new UpAir());
                } else if (direction.y.Equals(TiltDirection.Down)) {
                    changeState(new DownAir());
                } else if (direction.x.Equals(TiltDirection.Forward) || direction.x.Equals(TiltDirection.Neutral)) {
                    if (falling) {
                        changeState(new FallingFair());
                    } else {
                        changeState(new RisingFair());
                    }
                } 
            } else {
                if (direction.y.Equals(TiltDirection.Up)) {
                    changeState(new UpTilt());
                } else if (direction.y.Equals(TiltDirection.Down)) {
                    changeState(new DownTilt());
                } else if (direction.x.Equals(TiltDirection.Neutral) || direction.x.Equals(TiltDirection.Forward)) {
                    changeState(new Jab1());
                }
            }
        }
    }
}

public class CanAttack<T> : CanAttack where T : Attack {
    public override void checkCondition() {
        if (AtlasInputManager.getKeyPressed("Attack")) {
            changeState(getNewState<T>());
        }
    }
}
}