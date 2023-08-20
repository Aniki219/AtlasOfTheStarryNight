using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using System.Threading.Tasks;
using Transitions.DiveToad;

namespace States.DiveToad {
  public class Land : State {
    public Land() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {
        new ReturnToIdle().Pause()
      };
    }

    public async override Task StartState()
    {
      await base.StartState();
      deformer.startDeform(new Vector3(1.2f, 0.75f, 1.0f), 0.1f, 0.1f);
      await AtlasHelpers.WaitSeconds(0.1f);
      UnpauseTransition<ReturnToIdle>();         
    }

    public override void UpdateState()
    {
      base.UpdateState();
    }

    public async override Task ExitState()
    {
      await base.ExitState();
    }
  }
}