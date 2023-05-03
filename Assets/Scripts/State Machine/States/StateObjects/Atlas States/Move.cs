using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions;

namespace States {
  public class Move : State {
    public Move() {
      behaviors = new List<IStateBehavior>() {
        new MoveBehavior()
      };

      transitions = new List<IStateTransition>() {
        new CanAttack(),
        new CanBroom(),
        new CanJump(),
        new CanSlip(),
        new CanFall(),
        new CanLift(),
        new CanCrouch(),
      };
    }
  }
}