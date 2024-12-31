using System.Collections.Generic;
using UnityEngine;

namespace States {
  public class Carry : State {
    public Carry() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.CarryBehavior()
      };

      transitions = new List<IStateTransition>() {

      };
    }
  }
}