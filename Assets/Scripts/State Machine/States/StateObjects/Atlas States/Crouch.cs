using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class Crouch : State {
    public Crouch() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior()
          .CanTurnAround(false)
          .MoveSpeedMod(0.5f),
        new Behaviors.NovaBehavior().GainSpeed(NovaManager.GainSpeed.Hold),
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack<States.Attacks.DownTilt>().SkipWaitForExit(),
        new Transitions.CanJump<States.GroundJump>().Pause(),
        new Transitions.CanBroom(),
        new Transitions.CanSlip(),
        new Transitions.CanSlide().SkipWaitForExit(),
        new Transitions.CanFall(),
        new Transitions.CanUncrouch(),
      };
    }

    PlayerController pc;
    Vector3 colliderStartSize;
    Vector3 colliderStartOffset;

    public override async Task StartState()
    {
      await base.StartState();
      pc = (PlayerController)stateMachine;
      colliderManager.setActiveCollider("Crouching");

      //TODO I think we need to fix the animator for this to work
      //alwaysUpdate = true;
      if (!skipStartAnim) {
        performAnim();
      }     
    }

    async void performAnim() {
      PauseTransition<Transitions.CanUncrouch>();
      await AnimMapper.awaitClip<States.Crouch>();
      UnpauseTransition<Transitions.CanUncrouch>();
    }

    public override void UpdateState()
    {
      base.UpdateState();
      if (Mathf.Abs(controller.velocity.x) > 0 && AtlasHelpers.Sign(controller.velocity.x) != pc.facing) {
        anim.SetFloat("animDir", -1f);
      } else {
        anim.SetFloat("animDir", 1f);
      }
    }

    public override async Task ExitState()
    {
      anim.SetFloat("animDir", 1f);
      UnpauseTransition<Transitions.CanJump<States.GroundJump>>(); //cant becaue transitions go on Update phase
      colliderManager.setActiveCollider("Standing");
      
      GetBehavior<Behaviors.MoveBehavior>().MoveSpeedMod(1.0f);
      SetAnimation(StateMachine.Phase.Exit);
      if (controller.isGrounded()) {
        await AtlasHelpers.WaitSeconds(AtlasHelpers.FindAnimation(anim, "CrouchExit").length);
      }
      await base.ExitState();
    }
  }
}