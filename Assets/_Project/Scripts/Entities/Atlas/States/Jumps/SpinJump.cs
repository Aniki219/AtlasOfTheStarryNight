using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace States {
  public class SpinJump : Jump {
    private PlayerController pc;

    public SpinJump() : base() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior(),
        new Behaviors.JumpBehavior()
          .JumpHeight(2)
          .JumpDistance(getJumpDistance())
          .Lockout(Behaviors.JumpBehavior.LockoutType.TwiceApex),
        new Behaviors.NovaBehavior().GainSpeed(NovaManager.GainSpeed.HoldCharged)
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanBroom<States.QuickBroom>(),
        new Transitions.CanBroom<States.NovaDash>(),
        new Transitions.CanJump<States.DoubleJump>(),
        new Transitions.CanFall(),
        new Transitions.CanLand(),
        new Transitions.CanSlip(),
        new Transitions.CanWallSlide(),
      };
    }

    public float getJumpDistance()
    {
      pc = (PlayerController)stateMachine;
      float jumpDistance = 6;
      if (AtlasInputManager.getAxis("Dpad").getDirection().x != TiltDirection.Neutral) {
        jumpDistance += AtlasInputManager.getAxis("Dpad").getDirection().x == TiltDirection.Forward ? 2 : -2;
      }
      return jumpDistance;
    }

    public async override Task ExitState()
    {
      await base.ExitState();
    }
  }
}