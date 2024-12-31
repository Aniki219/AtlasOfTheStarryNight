using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace States {
  public class Land : AtlasState {
    protected float duration = 0;

    public Land() {
      transitions = new List<IStateTransition>() {
        new Transitions.CanSlip()
      };
    }

    public async override Task StartState()
    {
      await base.StartState();

      if (duration > 0) {
        particleMaker.createDust(true);
        controller.velocity = Vector3.zero;
        
        await AtlasHelpers.WaitSeconds(duration);
      }
      stateMachine.ChangeState(new Idle());
    }
  }

  public class AttackLand : Land {

    public async override Task StartState()
    {
      AnimationClip attackLandClip = FindStatePhaseClip(StateMachine.Phase.Start);
      duration = attackLandClip.length - 0.1f;

      await base.StartState();
    }
  }
}