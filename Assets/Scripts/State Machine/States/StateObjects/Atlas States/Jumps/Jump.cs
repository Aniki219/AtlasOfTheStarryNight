using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Transitions;
using Behaviors;

namespace States {
  public abstract class Jump : State {
    protected List<IStateTransition> getJumpTransitions() {
      return new List<IStateTransition>() {
        new CanAttack(),
        new CanBroom(),
        new CanJump<DoubleJump>(),
        new CanFall(),
        new CanLand(),
        new CanSlip(),
        new CanWallSlide(),
        new CanTurnAround(),
      };
    }

    public virtual void CreateJumpEffect() {}
  }

  public class GroundJump : Jump {
    public GroundJump() {
      behaviors = new List<IStateBehavior>() {
        new MoveBehavior(),
        new JumpBehavior()
          .VariableJump()
          .JumpHeight(4f)
      };

      transitions = getJumpTransitions();
    }

    public override void CreateJumpEffect() {
      particleMaker.createDust(true);
    }
  }

  public class PostJump : Jump {
    public PostJump() : base() {
      behaviors = new List<IStateBehavior>() {
        new MoveBehavior()
      };

      transitions = getJumpTransitions();
    }
  }
}