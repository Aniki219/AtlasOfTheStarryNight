using System.Collections.Generic;
using UnityEngine;

namespace States {
  public class Attack : State {
    public Attack() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.AttackBehavior().ResetVelocity()
      };

      transitions = new List<IStateTransition>() {
      };
    }
  }
}