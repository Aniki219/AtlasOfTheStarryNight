using System.Collections.Generic;
using UnityEngine;

namespace States {
  public abstract class AirAttack : State {
    public AirAttack() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior(false),
        new Behaviors.AttackBehavior(),
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanLand(true)
      };
    }
  }

  namespace AirialAttacks {
    public class RisingFair : AirAttack {}
    public class FallingFair : AirAttack {}
    public class UpAir : AirAttack {}
    public class DownAir : AirAttack {}
  }
}