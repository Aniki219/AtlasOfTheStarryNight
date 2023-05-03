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

  public class AttackLand : State {
    public AttackLand() {
      AnimationClip attackLandClip = AnimMapper.getClip<States.AttackLand>();
      
      behaviors = new List<IStateBehavior>() {
        new Behaviors.LandBehavior((int)(attackLandClip.length * 1000)),
      };

      transitions = new List<IStateTransition>() {

      };
    }
  }
}