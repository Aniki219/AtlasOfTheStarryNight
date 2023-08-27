using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class Hurt : AtlasState {
    public bool reset;
    public int damage;

    public Hurt() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {

      };
    }

    public Hurt Resetting() {
      reset = true;
      return this;
    }

    public async override Task StartState()
    {
      await base.StartState();

      pc.setInvulnerable(1.75f);

      SoundManager.Instance.playClip(reset ? "hurt2" : "hurt");
      if (damage > 0) { ResourceManager.Instance.takeDamage(damage); }

      controller.lockPosition = true;
      await AtlasHelpers.WaitSeconds(0.1f);
      controller.lockPosition = false;

      controller.velocity.y = 4f;
      controller.velocity.x = -pc.moveSpeed/4f * pc.facing;
      
      Camera.main.GetComponent<cameraController>().StartShake();
    }

    public override void OnAnimationEnd()
    {
      if (reset) {
        stateMachine.changeState(new Reset());
      } else {
        stateMachine.changeState(new Idle());
      }
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