using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class Slide : State {
    IEaser<float> slideVelocityEaser;

    public Slide() {
      behaviors = new List<IStateBehavior>() {
        new Behaviors.NovaBehavior().GainSpeed(NovaManager.GainSpeed.Fast),
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanJump<States.SpinJump>(),
        new Transitions.CanFall()
      };
    }

    public async override Task StartState() {
      await base.StartState();
      colliderManager.setActiveCollider("Crouching");
      float dir = AtlasInputManager.getAxis("Dpad").getValue().x;
      if (Mathf.Abs(dir) > 0.1f) stateMachine.setFacing(dir);
      slideVelocityEaser = new CompositeEaser(
                              //new Easer(2, 10f, 0.2f, Ease.InOutQuad),
                              new Easer(10, 1f, 0.5f, Ease.OutQuad)
                            );
      PauseTransition<Transitions.CanJump<States.SpinJump>>(0.05f);
      particleMaker.createDust(true);
      await Task.Yield();
    }

    public override void UpdateState() {
      base.UpdateState();
      if (slideVelocityEaser.isComplete) {
          controller.setXVelocity(0);
          stateMachine.changeState(new States.Crouch().SkipStartAnim());
      } else {
          controller.setXVelocity(slideVelocityEaser.Update());
      }
    }

    public async override Task ExitState()
    {
      colliderManager.setActiveCollider("Standing");
      controller.setXVelocity(Mathf.Clamp(controller.velocity.x, -6, 6), characterController.VelocityType.Absolute);

      await base.ExitState();
      await Task.Yield();
    }
  }
}