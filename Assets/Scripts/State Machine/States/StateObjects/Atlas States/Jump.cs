using System.Collections.Generic;
using UnityEngine;

namespace States {
  public class Jump : State {
    public Jump() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior(),
        new Behaviors.JumpBehavior(new Vector2(0, 8.5f), true),
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanBroom(),
        new Transitions.CanJump(),
        new Transitions.CanFall(),
        new Transitions.CanSlip(),
        new Transitions.CanWallSlide(),
      };
    }
  }

  public class PostJump : Jump {
    public PostJump() : base() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior()
      };
    }
  }
}