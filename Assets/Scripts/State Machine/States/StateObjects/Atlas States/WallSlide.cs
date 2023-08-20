using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Behaviors;
using Transitions;

namespace States
{
  public class WallSlide : AtlasState {
    PlayerController pc;

    public WallSlide() {
      behaviors = new List<IStateBehavior>() {
        new NovaBehavior().GainSpeed(NovaManager.GainSpeed.SlowMinus),
      };

      transitions = new List<IStateTransition>() {
        new CanJump<WallJump>(),
        new CanLand(),
        new CanUnwallSlide(),
        new CanBroom(),
      };
    }

    protected override void attachComponents() {
      base.attachComponents();
      pc = (PlayerController)stateMachine;
    }

    public override async Task StartState() {
      await base.StartState();
      spriteController.dustTrail.SetActive(true);
      PauseTransition<CanUnwallSlide>(0.15f);
    }

    public override void UpdateState() {
      base.UpdateState();
      Mathf.Clamp(controller.velocity.y, pc.maxWallSlideVel, Mathf.Infinity);
    }

    public override async Task ExitState() {
      await base.ExitState();
      spriteController.dustTrail.SetActive(false);
    }
  }
}