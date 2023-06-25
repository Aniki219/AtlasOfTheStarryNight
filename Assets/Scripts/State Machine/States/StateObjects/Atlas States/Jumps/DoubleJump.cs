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

    public async override Task StartState(StateMachine stateMachine)
    {
      await base.StartState(stateMachine);
      if (AtlasInputManager.getAxisState("Dpad").x * controller.velocity.x < 0) {
        controller.velocity.x = 0;
      }
      playAnim();      
    }

    private async void playAnim() {
      PauseTransition<Transitions.CanFall>();
      anim.SetBool("isDoubleJumping", true);
      await AnimMapper.awaitClip<DoubleJump>();
      UnpauseTransition<Transitions.CanFall>();
    }

    public override async Task ExitState()
    {
      anim.SetBool("isDoubleJumping", false);
      await base.ExitState();
    }
  }
}