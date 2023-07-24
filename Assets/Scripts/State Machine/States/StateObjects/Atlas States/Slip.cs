using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace States {
  public class Slip : State {
    public Slip() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.SlipBehavior(),
        new Behaviors.NovaBehavior().GainSpeed(NovaManager.GainSpeed.Fast),
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanBroom(),
        new Transitions.CanFall(),

        new Transitions.CanJump<States.GroundJump>(),
        new Transitions.CanJump<States.SpinJump>(),
      };
    }

    public override async Task StartState()
    {
      await base.StartState();
      broomEffectsController.SetSlipSparks(true);
    }

    public override async Task ExitState()
    {
      broomEffectsController.SetSlipSparks(false);
      await base.ExitState();
    }
  }
}