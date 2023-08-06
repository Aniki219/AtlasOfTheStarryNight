﻿using UnityEngine;
using System;

namespace Transitions {
    public class RunIdle : IStateTransition {
        public override void checkCondition() {
            if (Mathf.Abs(state.controller.velocity.x) < 0.01f) {
                changeState(new States.Idle());
            } else {
                changeState(new States.Run());
            }
        }
    }
}