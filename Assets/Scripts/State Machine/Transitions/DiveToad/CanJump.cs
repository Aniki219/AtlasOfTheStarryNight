using UnityEngine;
using System;
using States.DiveToad;

namespace Transitions.DiveToad {
    
    public class CanJump : IStateTransition {
        private EnemyController ec;

        public override void Attach(State state) {
            ec = (EnemyController)state.stateMachine;
            base.Attach(state);
        }

        public override void checkCondition() {
            if (!ec.awakened || !ec.canAct()) return;
            float dx = gameManager.Instance.player.transform.position.x - state.transform.position.x;
            if (Mathf.Abs(dx) > 2.5f) changeState<Jump>();
        }
    }
}
