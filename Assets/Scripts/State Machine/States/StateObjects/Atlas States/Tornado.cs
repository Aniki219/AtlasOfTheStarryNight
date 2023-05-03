using System.Collections.Generic;
using UnityEngine;

namespace States {
  public class Tornado : State {
    public Tornado() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {

        new Transitions.CanJump(),

      };
    }
  }
}