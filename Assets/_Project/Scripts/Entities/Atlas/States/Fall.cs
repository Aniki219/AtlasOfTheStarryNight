using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Behaviors;
using Transitions;

namespace States {
  public class Fall : State {
    public Fall() {
      behaviors = new List<IStateBehavior>() {
        new MoveBehavior()
      };

      transitions = new List<IStateTransition>() {
        new CanAttack(),
        new CanBroom(),
        new CanJump<States.DoubleJump>(),
        new CanSlip(),
        new CanLand(),
        new CanWallSlide(),
        new CanTurnAround(),
      };
    }

    public override async Task ExitState()
    {
      await base.ExitState();
    }
  }
}