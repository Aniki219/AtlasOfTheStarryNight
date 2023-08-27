using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class Bonk : AtlasState {
    public Bonk() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {

      };
    }

    public async override Task StartState()
    {
      await base.StartState();
      SoundManager.Instance.playClip("bonk");
      pc.createStars(transform.position);
      
      deformer.startDeform(new Vector3(.25f, 1.1f, 1), 0.1f, .35f, Vector2.right * pc.facing);
      
      controller.lockPosition = true;
      await AtlasHelpers.WaitSeconds(0.1f);
      controller.lockPosition = false;

      controller.velocity.y = 4f;
      controller.velocity.x = -pc.moveSpeed/4f * pc.facing;
      
      AtlasEventManager.Instance.BonkEvent();
      
      Camera.main.GetComponent<cameraController>().StartShake();
    }

    public override void OnAnimationEnd()
    {
      stateMachine.changeState(new Idle());
    }

    public override void UpdateState()
    {
      base.UpdateState();

      if (!pc.isGrounded())
      {
        controller.velocity.x = -pc.moveSpeed/4f * pc.facing;
      }
    }

    public async override Task ExitState()
    {
      if (pc.isGrounded())
      {
        controller.velocity.x = 0;
      }
      await base.ExitState();
    }
  }
}