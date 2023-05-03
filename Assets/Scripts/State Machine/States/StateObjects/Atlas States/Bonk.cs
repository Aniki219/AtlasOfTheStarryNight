using System.Collections.Generic;
using UnityEngine;

namespace States {
  public class Bonk : State {
    public Bonk() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.BonkBehavior()
      };

      transitions = new List<IStateTransition>() {

      };
    }
  }
}