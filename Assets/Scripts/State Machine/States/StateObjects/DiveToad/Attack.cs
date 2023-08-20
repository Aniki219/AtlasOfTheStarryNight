using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions.DiveToad;
using System.Threading.Tasks;

namespace States.DiveToad {
  public class Attack : State {
    public Attack() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {
        new CanLand(),
        new CanFall<AttackFall>(),
        new CanHurt(),
      };
    }

    public async override Task StartState()
    {
      await base.StartState();
      controller.setXVelocity(4f);
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