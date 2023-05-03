using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class DoubleJump : State {
    public DoubleJump() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.MoveBehavior(),
        new Behaviors.JumpBehavior(new Vector2(0, 6)),
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanBroom(),
        new Transitions.CanJump(),
        new Transitions.CanSlip(),
        new Transitions.CanLift(),
        new Transitions.CanFall(),
      };
    }

    public async override Task StartState(StateMachine stateMachine, bool wasActive = false)
    {
      await base.StartState(stateMachine, wasActive);
      PauseTransition<Transitions.CanFall>();
      await AnimMapper.awaitClip<DoubleJump>();
      UnpauseTransition<Transitions.CanFall>();
    }

    public override async Task ExitState()
    {
      await base.ExitState();
      anim.SetBool("isDoubleJumping", false);
    }
  }
}