using UnityEngine;
using System;
using States.DiveToad;

namespace Transitions.DiveToad {
    
    public class ReturnToIdle : IStateTransition {
        public override void checkCondition() {
            changeState(new Idle());
        }
    }
}
