using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace States
{
  public class WallSlide : State {
    playerController pc;

    public WallSlide() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanJump(),
        new Transitions.CanLand(),
        new Transitions.CanUnwallSlide(),
        new Transitions.CanBroom(),
      };
    }

    protected override void attachComponents() {
      base.attachComponents();
      pc = (playerController)stateMachine;
    }

    public override async Task StartState(StateMachine stateMachine, bool wasActive = false) {
      await base.StartState(stateMachine, wasActive);
      anim.SetBool("wallSlide", true);
      spriteController.dustTrail.SetActive(true);
    }

    public override async void UpdateState() {
      base.UpdateState();
      Mathf.Clamp(controller.velocity.y, pc.maxWallSlideVel, Mathf.Infinity);
    }

    public override async Task ExitState() {
      await base.ExitState();
      anim.SetBool("wallSlide", false);
      spriteController.dustTrail.SetActive(false);
    }
  }
}