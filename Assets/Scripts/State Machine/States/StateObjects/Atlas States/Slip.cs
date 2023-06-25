using System.Collections.Generic;
using UnityEngine;

namespace States {
  public class Slip : State {
    public Slip() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.SlipBehavior()
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanBroom(),
        new Transitions.CanJump<States.SpinJump>(),
        new Transitions.CanFall(),
      };
    }
  }
}