using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace States {
  public class Fall : State {
    public Fall() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior()
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanBroom(),
        new Transitions.CanJump(),
        new Transitions.CanLand(),
        new Transitions.CanWallSlide(),
      };
    }

    public override async Task StartState(StateMachine stateMachine, bool wasActive = false)
    {
      await base.StartState(stateMachine, wasActive);
      anim.SetBool("isFalling", !anim.GetBool("isDoubleJumping"));
      anim.SetBool("isJumping", false);
    }

    public override async Task ExitState()
    {
      await base.ExitState();
      anim.SetBool("isFalling", false);
    }
  }
}