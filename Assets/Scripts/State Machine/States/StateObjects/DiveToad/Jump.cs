using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions.DiveToad;
using System.Threading.Tasks;

namespace States.DiveToad {
  public class Jump : State {
    public Jump() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {
        new CanLand(),
        new CanFall<Fall>(),
        new CanHurt(),
      };
    }

    public async override Task StartState()
    {
      await base.StartState();
      deformer.startDeform(new Vector3(0.9f, 1.1f, 1.0f), 0.1f, 0.1f);
      controller.setXVelocity(2.5f);
      controller.velocity.y = 3f;
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