using UnityEngine;
using System;
using System.Threading.Tasks;

namespace Transitions {
    public class CanJump<T> : IStateTransition where T : States.Jump {
        PlayerController pc;

        public override void Attach(State state) {
            base.Attach(state);
            pc = (PlayerController)state.stateMachine;    
        }

        public override void checkCondition() {
            if (AtlasInputManager.getKeyPressed("Jump") && state.controller.CheckVertDist(0.3f)) {
                changeState(getNewState<T>());
            }
        }
    }
}