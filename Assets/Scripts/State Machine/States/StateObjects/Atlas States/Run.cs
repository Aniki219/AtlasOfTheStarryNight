using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions;

namespace States {
  public class Run : State {
    public Run() {
      behaviors = new List<IStateBehavior>() {
        new MoveBehavior()
      };

      transitions = new List<IStateTransition>() {
        new CanAttack(),
        new CanBroom(),
        new CanJump<States.GroundJump>(),
        new CanSlip(),
        new CanFall(),
        new CanLift(),
        new CanCrouch(),
        new RunIdle()
      };
    }
  }

  public class Idle : Run {
    public Idle() : base() {}
  }
}