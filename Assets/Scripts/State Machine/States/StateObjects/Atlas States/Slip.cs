using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Transitions;

namespace States {
  public class Slip : AtlasState {
    readonly float timeToSpinJump = 0.1f;
    
    public Slip() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.NovaBehavior().GainSpeed(NovaManager.GainSpeed.Fast),
      };

      transitions = new List<IStateTransition>() {
        new CanBroom(),
        new CanFall(),

        //new CanJump<GroundJump>(),
        new CanJump<SpinJump>().Pause(),
      };
    }

    public override async Task StartState()
    {
      await base.StartState();
      broomEffectsController.SetSlipSparks(true);

      pc.setFacing(AtlasHelpers.Sign(controller.collisions.getGroundSlope().x));

      controller.velocity.x = 0;
      
      await Task.Yield();
    }

    public override void UpdateState()
    {
      base.UpdateState();
      if (controller.collisions.getGroundSlope().y > controller.maxSlope) {
          pc.ChangeState(new Idle());
      }
      if (StateTime() > timeToSpinJump) {
          GetTransition<CanJump<SpinJump>>().Unpause();
      }
    }

    public override async Task ExitState()
    {
      broomEffectsController.SetSlipSparks(false);
      await base.ExitState();
    }
  }
}