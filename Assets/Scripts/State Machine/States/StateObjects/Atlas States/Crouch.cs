using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class Crouch : State {
    public Crouch() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior(false, 0.5f)
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanBroom(),
        new Transitions.CanSlip(),
        new Transitions.CanSlide(),
        new Transitions.CanFall(),
        new Transitions.CanUncrouch(),
      };
    }

    playerController pc;
    Vector3 colliderStartSize;
    Vector3 colliderStartOffset;

    public override async Task StartState(StateMachine stateMachine, bool wasActive = false)
    {
      await base.StartState(stateMachine, wasActive);
      pc = (playerController)stateMachine;

      anim.SetBool("isCrouching", true);
      
      colliderStartSize = boxCollider.size;
      colliderStartOffset = boxCollider.offset;
      
      boxCollider.size = Vector2.Scale(colliderStartSize, new Vector3(1.0f, 0.5f));
      boxCollider.offset = Vector2.up * (colliderStartOffset.y - colliderStartSize.y * 0.25f);
      
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
      await base.ExitState();
      anim.SetBool("isCrouching", false);
      anim.SetFloat("animDir", 1f);
      boxCollider.size = colliderStartSize;
      boxCollider.offset = colliderStartOffset;
      await AnimMapper.awaitClip<States.Crouch>(AnimMapper.ClipType.ExitClip);
    }
  }
}