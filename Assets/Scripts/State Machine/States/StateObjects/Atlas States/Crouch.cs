using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Transitions;

namespace States {
  public abstract class CrouchBase : AtlasState {
    public CrouchBase() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior()
          .CanTurnAround(false)
          .MoveSpeedMod(0.5f),
        new Behaviors.NovaBehavior().GainSpeed(NovaManager.GainSpeed.Hold),
      };

      transitions = new List<IStateTransition>() {
        new CanAttack<Attacks.DownTilt>().SkipWaitForExit(),
        new CanJump<GroundJump>().Pause(),
        new CanBroom(),
        new CanSlip(),
        new CanSlide().SkipWaitForExit(),
        new CanFall(),
        new CanUncrouch(),
        new CrouchCrawl().SkipWaitForExit(),
      };
    }

    public override async Task ExitState()
    {
      anim.SetFloat("animDir", 1f);
      UnpauseTransition<CanJump<GroundJump>>(); //cant becaue transitions go on Update phase
      colliderManager.setActiveCollider("Standing");
      
      await base.ExitState();
    }
  }

  public class Crouch : CrouchBase {
    
    public override async Task StartState()
    {
      await base.StartState();
      colliderManager.setActiveCollider("Crouching");
      controller.velocity.x = 0;

      //TODO I think we need to fix the animator for this to work
      //alwaysUpdate = true;
      if (!skipStartAnim) {
        performAnim();
      }     
    }

    private async void performAnim() {
      PauseTransition<CanUncrouch>();
      await WaitForPhaseAnimation(StateMachine.Phase.Start);
      UnpauseTransition<CanUncrouch>();
    }

    public override async Task ExitState() {
      GetBehavior<Behaviors.MoveBehavior>().MoveSpeedMod(1.0f);
      SetAnimation(StateMachine.Phase.Exit);
      if (controller.IsGrounded()) {
        await AtlasHelpers.WaitSeconds(AtlasHelpers.FindAnimation(anim, "CrouchExit").length);
      }
    }
  }

  public class Crawl : CrouchBase {

    public override void UpdateState()
    {
      base.UpdateState();
      if (Mathf.Abs(controller.velocity.x) > 0 && AtlasHelpers.Sign(controller.velocity.x) != pc.facing) {
        anim.SetFloat("animDir", -1f);
      } else {
        anim.SetFloat("animDir", 1f);
      }
    }
  }
}