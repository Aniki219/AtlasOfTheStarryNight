using UnityEngine;
using System;

namespace Transitions {
    public class CanTurnAround<T> : CanTurnAround where T : State {
        protected override void turnAround() {
            changeState(getNewState<T>());
        }
    }
    
    public class CanTurnAround : IStateTransition {
        protected PlayerController pc;

        public override void Attach(State state)
        {
            base.Attach(state);
            pc = (PlayerController)state.stateMachine;
        }

        public override void checkCondition() {
            if (!Mathf.Approximately(AtlasInputManager.getAxis("Dpad").getSignX(), 0) && 
                !AtlasHelpers.SameSign(state.stateMachine.facing, AtlasInputManager.getAxis("Dpad").getSignX())) {
                    turnAround();
            }
        }

        protected virtual void turnAround() {
            pc.setFacing(pc.facing * -1);
        }
    }
}
