using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions.DiveToad;
using System.Threading.Tasks;

namespace States.DiveToad {
  public class Fall : State {
    public Fall() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {
        new CanLand(),
        new CanHurt(),
      };
    }
  }

  public class AttackFall : Fall {}
}