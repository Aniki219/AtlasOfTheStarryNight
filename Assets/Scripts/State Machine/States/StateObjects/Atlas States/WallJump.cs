using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class WallJump : Jump {
    playerController pc;

    public WallJump() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanFall(),
        //Cancellables...Broom, Attack?
      };
    }

    protected override void attachComponents() {
      base.attachComponents();
      pc = (playerController)stateMachine;
    }

    public override async Task StartState(StateMachine stateMachine) {
      await base.StartState(stateMachine);
      anim.SetBool("wallBlast", true);
      GameObject explosion = gameManager.createInstance("Effects/Explosions/wallBlast", 
                              transform.position + new Vector3(-0.80f * pc.facing, 0.23f, 0));
      explosion.transform.localScale = sprite.localScale;

      //TODO: not a player setting
      if (pc.screenShake) { Camera.main.GetComponent<cameraController>().StartShake(0.2f, 0.15f); }

      SoundManager.Instance.playClip("wallBlast");
      anim.SetBool("isJumping", false);
      anim.SetBool("isFalling", false);
      anim.SetBool("wallSlide", false);
      anim.SetBool("wallBlast", true);

      controller.canGravity = false;
      controller.canMove = false;

      await AtlasHelpers.WaitSeconds(pc.wallBlastDelay);

      pc.flipHorizontal();

      controller.canGravity = true;
      controller.canMove = true;

      anim.SetBool("isJumping", true);
      anim.SetBool("wallBlast", false);

      controller.velocity.y = pc.wallJumpVelocity;

      float startTime = Time.time;
      controller.velocity.x = pc.wallJumpVelocity * pc.facing;
      await AtlasHelpers.WaitSeconds(0.12f);
      stateMachine.changeState(new States.PostJump());
    }

    public override async Task ExitState() {
      await base.ExitState();
      anim.SetBool("wallBlast", false);
    }
  }
}