using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions.DiveToad;
using System.Threading.Tasks;

namespace States.DiveToad {
  public class Hurt : State {
    HitBox hitbox;

    public Hurt() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {

      };
    }

    public Hurt HitBox(HitBox _hitBox) {
      hitbox = _hitBox;
      return this;
    }

    public async override Task StartState()
    {
      await base.StartState();
      ((EnemyController)stateMachine).FaceTowardsPlayer();
      float kbStrength = hitbox.knockback ? 2.5f : 1.5f;
      float dx = hitbox.kbDir.x;
      float dy = hitbox.kbDir.y;

      if (hitbox.explosive)
      {
          Vector2 dir = (transform.position - hitbox.position).normalized;
          dx = dir.x;
          dy = dir.y;
      }
      controller.velocity.x = kbStrength * dx;
      controller.velocity.y = kbStrength * 1.5f * dy;

      //TODO: await hitbox.hitStun;
      await AtlasHelpers.WaitSeconds(0.4f);

      stateMachine.ChangeState(new Idle());
    }

    public override void UpdateState()
    {
      base.UpdateState();
    }

    public async override Task ExitState()
    {
      await base.ExitState();
    }
  }
}