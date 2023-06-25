using System.Collections.Generic;
using UnityEngine;

namespace States {
  public abstract class Jump : State {
    protected List<IStateTransition> getJumpTransitions() {
      return new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanBroom(),
        new Transitions.CanJump<States.DoubleJump>(),
        new Transitions.CanFall(),
        new Transitions.CanSlip(),
        new Transitions.CanWallSlide(),
      };
    }
  }

  public class GroundJump : Jump {
    public GroundJump() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior(),
        new Behaviors.JumpBehavior()
          .VariableJump()
          .JumpHeight(5)
      };

      transitions = getJumpTransitions();
    }
  }

  public class PostJump : Jump {
    public PostJump() : base() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior()
      };

      transitions = getJumpTransitions();
    }
  }
}