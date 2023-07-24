using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class WallJump : Jump {
    PlayerController pc;

    public WallJump() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.NovaBehavior().GainSpeed(NovaManager.GainSpeed.Hold),
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanFall(),
        //Cancellables...Broom, Attack?
      };
    }

    protected override void attachComponents() {
      base.attachComponents();
      pc = (PlayerController)stateMachine;
    }

    public override async Task StartState() {
      await base.StartState();
      GameObject explosion = gameManager.createInstance("Effects/Explosions/wallBlast", 
                              transform.position + new Vector3(-0.80f * pc.facing, 0.23f, 0));
      explosion.transform.localScale = sprite.localScale;

      //TODO: not a player setting
      if (pc.screenShake) { Camera.main.GetComponent<cameraController>().StartShake(0.2f, 0.15f); }

      SoundManager.Instance.playClip("wallBlast");

      controller.canGravity = false;
      controller.canMove = false;
;
      await WaitForPhaseAnimation(StateMachine.Phase.Start);

      controller.canGravity = true;
      controller.canMove = true;

      controller.velocity.y = pc.wallJumpVelocity;
      controller.velocity.x = pc.wallJumpVelocity * -pc.facing;

      await AtlasHelpers.WaitSeconds(0.01f);
      pc.flipHorizontal();
      await AtlasHelpers.WaitSeconds(0.12f);
      stateMachine.changeState(new States.PostJump().RemoveTransition<Transitions.CanTurnAround>());
    }

    public override async Task ExitState() {
      await base.ExitState();
    }
  }
}