using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class DoubleJump : Jump {
    public DoubleJump() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior(),
        new Behaviors.JumpBehavior().JumpHeight(2f),
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanBroom(),
        new Transitions.CanSlip(),
        new Transitions.CanFall(),
        new Transitions.CanLand(),
        new Transitions.CanWallSlide(),
      };
    }

    public override void CreateJumpEffect() {
      particleMaker.createEffect("DoubleJumpBurst", transform.position);
    }

    public async override Task StartState()
    {
      await base.StartState();
      if (AtlasInputManager.getAxis("Dpad").getValue().x * controller.velocity.x < 0) {
        controller.velocity.x = 0;
      }
      playAnim();      
    }

    private async void playAnim() {
      PauseTransition<Transitions.CanFall>();
      await WaitForPhaseAnimation(StateMachine.Phase.Start);
      UnpauseTransition<Transitions.CanFall>();
    }

    public override async Task ExitState()
    {
      await base.ExitState();
    }
  }
}