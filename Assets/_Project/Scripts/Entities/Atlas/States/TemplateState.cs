using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions;
using System.Threading.Tasks;

namespace States {
  public class TemplateState : State {
    public TemplateState() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {

      };
    }

    public async override Task StartState()
    {
      await base.StartState();
    }

    public override void UpdateState()
    {
      base.UpdateState();
    }

    public async override Task ExitState()
    {
      await base.ExitState();
    }
  }
}