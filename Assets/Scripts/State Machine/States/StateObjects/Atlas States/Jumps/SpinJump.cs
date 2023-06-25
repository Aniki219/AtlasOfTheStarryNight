using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace States {
  public class SpinJump : Jump {
    public SpinJump() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior(),
        new Behaviors.JumpBehavior()
          .JumpHeight(2)
          .JumpDistance(6)
          .Lockout(Behaviors.JumpBehavior.LockoutType.Apex)
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanBroom(),
        new Transitions.CanJump<States.DoubleJump>(),
        new Transitions.CanFall(),
        new Transitions.CanLand(),
        new Transitions.CanSlip(),
        new Transitions.CanWallSlide(),
      };
    }

    public async override Task StartState(StateMachine stateMachine)
    {
      this.stateMachine = stateMachine;
      base.attachComponents();
      anim.SetBool("isJumping", false);
      anim.SetBool("isSpinJumping", true);
      await base.StartState(stateMachine);
    }

    public async override Task ExitState()
    {
      await base.ExitState();
      anim.SetBool("isSpinJumping", false);
    }
  }
}