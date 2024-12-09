using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Behaviors {
    [Serializable]
    public class TemplateBehavior : IStateBehavior
    {
        public async override Task StartBehavior() {
            await Task.Yield();
        }

        public override void UpdateBehavior() {
            
        }

        public override async Task ExitBehavior() {
            await Task.Yield();
        }
    }
}