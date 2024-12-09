using System.Collections.Generic;
using UnityEngine;
using Behaviors;
using Transitions;
using System.Threading.Tasks;

namespace States {
  public class WallRide : AtlasState {
    public WallRide(BroomBehavior previousBroomBehavior) {
      behaviors = new List<IStateBehavior>() {
        previousBroomBehavior.CanWallRide(),
      };

      transitions = new List<IStateTransition>() {
        new IsNotWallRide(),
      };
    }

    public async override Task StartState()
    {
      await base.StartState();
    }

    public override void UpdateState()
    {
      base.UpdateState();

        // if (cc.velocity.y < 0) {
        //   if (targetBroomSpeed < 9) {
        //     targetBroomSpeed += 2*Time.deltaTime;
        //   } else {
        //     if (targetBroomSpeed < 13) {
        //       broomEffectsController.createNovaAirPuff();
        //       targetBroomSpeed = 13;
        //     }
        //   }
        // }
        // timeStoppedWallriding = Mathf.Infinity;
        // correctionTime += 0.5f * Time.deltaTime;
    }

    public async override Task ExitState()
    {
      await base.ExitState();
    }
  }

  public class WallGrind : WallRide {
    public WallGrind(BroomBehavior previousBroomBehavior) : base(previousBroomBehavior) {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {
        new IsNotWallRide(),
      };
    }

    //astate.broomEffectsController.SetSlipSparks(false);
  }
}