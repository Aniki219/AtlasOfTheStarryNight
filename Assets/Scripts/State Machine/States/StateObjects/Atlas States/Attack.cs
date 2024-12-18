using System.Collections.Generic;
using UnityEngine;
using Transitions;
using Behaviors;
using System.Threading.Tasks;

namespace States {
  public abstract class Attack : State {
    float clipLength = Mathf.Infinity;

    public Attack() {
      behaviors = new List<IStateBehavior>() {
        new AttackBehavior().ResetVelocity()
      };

      transitions = new List<IStateTransition>() {
      };
    }

    public async override Task StartState()
    {
      await base.StartState();
    }

    public override void UpdateState()
    {
      base.UpdateState();
    }

    public override void OnAnimationEnd() {
      if (!controller.IsGrounded()) {
        if (controller.velocity.y > 0) {
          stateMachine.ChangeState(new States.PostJump());
          return;
        }
        stateMachine.ChangeState(new States.Fall());
        return;
      }
      //Grounded
      if (AtlasInputManager.getAxis("Dpad").getDirection().y.Equals(TiltDirection.Down)) {
        stateMachine.ChangeState(new States.Crouch().SkipStartAnim());
        return;
      }
      if (AtlasInputManager.getAxis("Dpad").getDirection().x.Equals(TiltDirection.Neutral)) {
        stateMachine.ChangeState(new States.Run());
        return;
      }
      stateMachine.ChangeState(new States.Idle());
    }
  }

  public abstract class AirAttack : Attack {
    public AirAttack() {
      behaviors = new List<IStateBehavior>() {
        new MoveBehavior().CanTurnAround(false),
        new AttackBehavior(),
      };

      transitions = new List<IStateTransition>() {
        new CanLand<Land>(),
        new CanLand<AttackLand>(),
      };
    }

    public override void UpdateState()
    {
      base.UpdateState();
      if (StateTime() > 0.2f) {
        PauseTransition<CanLand<AttackLand>>();
      }
    }
  }

  namespace Attacks {
    public class UpTilt : Attack {}
    public class DownTilt : Attack {
      public override void OnAnimationEnd() {
        Debug.Log("Crouch Anim End");
        stateMachine.ChangeState(new States.Crouch().SkipStartAnim());
      }
    }
    public abstract class Jab : Attack {

      public async override Task StartState()
      {
        if (AtlasInputManager.getAxis("Dpad").getDirection().x.Equals(TiltDirection.Forward)) {
          controller.Move(10*Vector3.right*stateMachine.facing);
        }
        await base.StartState();
      }

      public override void UpdateState()
      {
        base.UpdateState();
      }
    }
    public class Jab1 : Jab {
      public Jab1() : base() {
        transitions.Add(new CanAttack<Jab2>().TransitionTime(0.25f));
      }
    }
    public class Jab2 : Jab {
      public Jab2() : base() {
        transitions.Add(new CanAttack<Jab3>().TransitionTime(0.25f));
      }
    }
    public class Jab3 : Jab {}
  }

  namespace AerialAttacks {
    public class RisingFair : AirAttack {}
    public class FallingFair : AirAttack {}
    public class UpAir : AirAttack {}
    public class DownAir : AirAttack {}
  }
}