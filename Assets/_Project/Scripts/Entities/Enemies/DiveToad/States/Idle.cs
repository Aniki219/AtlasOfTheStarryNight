using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions.DiveToad;
using System.Threading.Tasks;

namespace States.DiveToad {
  public class Idle : State {
    TPManager tpManager;
    EnemyController ec;

    public Idle() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {
        new CanAttack(),
        new CanFall<Fall>()
      };

      tpManager = new TPManager(this,
        new TPManager.TransitionProbability(new CanJump(), 0.5f)
      );
    }

    public override void Attach(StateMachine stateMachine)
    {
      base.Attach(stateMachine);
      ec = (EnemyController)stateMachine;
    }

    public async override Task StartState()
    {
      controller.velocity = Vector2.zero;
      await base.StartState();
    }

    public override void UpdateState()
    {
      Vector3 playerPosition = gameManager.Instance.player.transform.position;
      float dx = playerPosition.x - transform.position.x;
      float dy = transform.position.y - playerPosition.y;

      if (ec.playerSpotted) {
        ec.FaceTowardsPlayer();
      }

      if (ec.awakened && ec.canAct()) {
        ec.playerSpotted = Mathf.Abs(dx) <= 3.0f && dy > -1f && dy < 5f;

        if (!ec.playerSpotted) {
          randomlyTurnAround();
        }
      } else {
        if (Mathf.Abs(dx) <= 2.5f) {
            ec.awakened = true;
        }
      }

      tpManager.Update();
      base.UpdateState();
    }

    public void randomlyTurnAround() {
      if (Random.value > 0.5f) {
        ec.setFacing(ec.facing * -1);
      }
    }

    public async override Task ExitState()
    {
      await base.ExitState();
    }
  }
}