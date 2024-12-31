using UnityEngine;
using System;
using States.DiveToad;

namespace Transitions.DiveToad {
    public class CanAttack : IStateTransition {
        private EnemyController ec;

        public override void Attach(State state) {
            ec = (EnemyController)state.stateMachine;
            base.Attach(state);
        }

        public override void checkCondition() {
            if (!ec.awakened || !ec.playerSpotted || !ec.canAct()) return;
            float dx = gameManager.Instance.player.transform.position.x - state.transform.position.x;
            float dy = state.transform.position.y - gameManager.Instance.player.transform.position.y;

            if (Mathf.Abs(dx) <= 2.5f) {
                if (dy <= 1) {
                    changeState<Attack>();
                } else {
                    changeState<Jump>();
                }
            }
        }
    }
}
