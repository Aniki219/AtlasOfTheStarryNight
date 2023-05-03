using System.Collections.Generic;
using UnityEngine;

namespace States {
  public class Broom : State {
    public Broom() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.BroomBehavior()
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanJump(),
      };
    }
  }
}