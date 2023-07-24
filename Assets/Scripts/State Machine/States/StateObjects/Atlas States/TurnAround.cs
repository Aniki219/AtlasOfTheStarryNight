using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions;
using System.Threading.Tasks;

namespace States {
  public abstract class TurnAround : State {
    protected PlayerController pc;
    public TurnAround() {
      behaviors = new List<IStateBehavior>() {
        new MoveBehavior(),
      };

      transitions = new List<IStateTransition>() {

      };      
    }

    public override void Attach(StateMachine stateMachine)
    {
      pc = (PlayerController)stateMachine;
      base.Attach(stateMachine);
    }

    public async override Task StartState()
    {
      pc.setFacing(pc.facing * -1);
      particleMaker.createDust(true);
      await base.StartState();
    }

  }

  public class TurnAroundRun : TurnAround {
    public TurnAroundRun() : base() {
      transitions = Idle.defaultTransitions;
      //transitions.Remove(GetTransition<CanTurnAround<TurnAroundRun>>());
    }

    public async override Task StartState()
    {
      await base.StartState();
      _ = PauseTransitions(FindStatePhaseClip(StateMachine.Phase.Start).length, GetTransition<RunIdle>());
    }
  }
  public class HoldTurn : TurnAround {}
}