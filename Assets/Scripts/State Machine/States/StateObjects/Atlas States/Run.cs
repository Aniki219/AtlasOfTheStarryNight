using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions;

namespace States {
  public class Idle : AtlasState {
    public Idle() {
      behaviors = new List<IStateBehavior>() {
        new MoveBehavior(),
      };

      transitions = new List<IStateTransition>();
      transitions.AddRange(defaultTransitions.ToArray()); 
    }
  }

  public class Run : Idle {
    public Run() : base() {
      behaviors.Add(new NovaBehavior().GainSpeed(NovaManager.GainSpeed.Slow));

      transitions.Remove(GetTransition<CanTurnAround>());
      transitions.Add(new CanTurnAround<TurnAroundRun>());
    }
  }
}