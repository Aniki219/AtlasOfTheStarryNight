using System.Collections.Generic;
using UnityEngine;

namespace States {
  public class Hurt : State {
    public Hurt() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.BonkBehavior()
      };

      transitions = new List<IStateTransition>() {

      };
    }
  }
}