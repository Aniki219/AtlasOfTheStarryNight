using System.Collections.Generic;
using UnityEngine;

namespace States {
  public class Slide : State {
    public Slide() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.SlideBehavior()
      };

      transitions = new List<IStateTransition>() {

      };
    }
  }
}