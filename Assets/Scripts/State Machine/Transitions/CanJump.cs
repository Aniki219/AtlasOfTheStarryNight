using UnityEngine;
using System;
using System.Threading.Tasks;

namespace Transitions {
    public class CanJump<T> : IStateTransition where T : States.Jump {
        playerController pc;

        public override void attach(State state) {
            base.attach(state);
            pc = (playerController)state.stateMachine;    
        }

        public override void checkCondition() {
            if (AtlasInputManager.getKeyPressed("Jump")) {
                changeState((T) Activator.CreateInstance(typeof(T)));
            }
        }
    }
}