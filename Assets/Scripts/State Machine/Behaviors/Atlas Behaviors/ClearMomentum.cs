using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Behaviors {
    [Serializable]
    public class ClearMomentum : IStateBehavior
    {
        private bool onExit = false;

        public ClearMomentum OnExit() {
            onExit = true;
            return this;
        }

        public async override Task StartBehavior() {
            state.controller.momentum = Vector3.zero;
            await Task.Yield();
        }

        public override void UpdateBehavior() {
            
        }

        public override async Task ExitBehavior() {
            if (onExit) state.controller.momentum = Vector3.zero;
            await Task.Yield();
        }
    }
}