using System.Collections.Generic;
using UnityEngine;

namespace States {
  public class Land : State {
    public Land() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.LandBehavior(),
      };

      transitions = new List<IStateTransition>() {

      };
    }
  }

  public class AttackLand : Land {
    public AttackLand() {
      AnimationClip attackLandClip = AnimMapper.getClip<States.AttackLand>();
      
      behaviors = new List<IStateBehavior>() {
        new Behaviors.LandBehavior(attackLandClip.length-0.1f),
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanSlip(),
      };
    }
  }
}