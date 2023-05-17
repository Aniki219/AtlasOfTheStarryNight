using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class DoubleJump : State {
    public DoubleJump() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior(),
        new Behaviors.JumpBehavior(new Vector2(0, 6)),
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanBroom(),
        new Transitions.CanSlip(),
        new Transitions.CanFall(),
        new Transitions.CanWallSlide(),
      };
    }

    public async override Task StartState(StateMachine stateMachine, bool wasActive = false)
    {
      await base.StartState(stateMachine, wasActive);
      if (AtlasInputManager.getAxisState("Dpad").x * controller.velocity.x < 0) {
        controller.velocity.x = 0;
      }
      playAnim();      
    }

    private async void playAnim() {
      PauseTransition<Transitions.CanFall>();
      await AnimMapper.awaitClip<DoubleJump>();
      UnpauseTransition<Transitions.CanFall>();
    }

    public override async Task ExitState()
    {
      await base.ExitState();
      anim.SetBool("isDoubleJumping", false);
    }
  }
}