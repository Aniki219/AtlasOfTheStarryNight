using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class Broom : AtlasState {
    protected PlayerController pc;
    protected CharacterController cc;
    protected float targetBroomSpeed;
    private float currentBroomSpeed;
    protected float smoothTime;
    protected float steerSpeed;
    protected bool quickBroom;
    public bool requiresNova {get; protected set;}

    public Broom() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanJump<States.DoubleJump>(),
      };

      targetBroomSpeed = 8;
      steerSpeed = 2;
      quickBroom = false;
      smoothTime = 0;
    }

    public override async Task StartState() {
      await base.StartState();

      pc = (PlayerController)stateMachine;
      cc = controller;

      pc.setFacing(AtlasHelpers.Sign(AtlasInputManager.getAxis("Dpad").getValue().x));

      SoundManager.Instance.playClip("onBroom");

      controller.velocity = Vector3.zero;
      controller.canGravity = false;

      if (!quickBroom) {
        await AtlasHelpers.WaitSeconds(AnimMapper.getClip<States.Broom>().length);
      } else {
        //await AtlasHelpers.WaitSeconds(0.1f);
      }

      pc.resetAnimator();
      SetBroomEffects();
      SoundManager.Instance.playClip("broomLaunch");
    }

    public override void UpdateState() {
      base.UpdateState();

      if (StateTime() > 0.1f) {
        if (AtlasInputManager.getKeyPressed("Broom") || 
          (!AtlasInputManager.getKey("Broom") && AtlasInputManager.Instance.holdBroom))
        {
            stateMachine.changeState(new States.Fall());
            return;
        }
      }

      if ((pc.facing == -1) ? cc.collisions.getLeft() : cc.collisions.getRight()) {
        if (cc.collisions.hasNormWhere(norm => Mathf.Abs(Vector2.Dot(norm, Vector2.right)) > 0.8f) ||
          Mathf.Abs(Vector2.Dot(cc.collisions.getAverageNorm(), Vector2.right)) > 0.8f) {
              pc.startBonk();
              stateMachine.changeState(new States.Bonk());
          }
      }
      float vdir = (AtlasInputManager.getAxis("Dpad").pressedAfter(stateStartTime)) ? 
                      AtlasInputManager.getAxis("Dpad").getValue().y : 0;
      if (smoothTime == 0) {
        currentBroomSpeed = targetBroomSpeed;
      } else {
        currentBroomSpeed = EasingFunctions.EaseInQuint(4, targetBroomSpeed, Mathf.Clamp(StateTime()/smoothTime, 0, 1));
      }
      cc.velocity.y = steerSpeed * vdir;
      cc.velocity.x = currentBroomSpeed * pc.facing;
    }

    public override async Task ExitState() {
      await base.ExitState();

      pc.broomEffects.SetTrail(false);
      pc.broomEffects.SetSpeedCone(false);
      controller.canGravity = true;
      AtlasEventManager.Instance.BroomCancelEvent();

      await Task.Yield();
    }

    protected virtual void SetBroomEffects() {
      pc.broomEffects.SetTrail(true);
    }
  }

  public class QuickBroom : Broom {
    public QuickBroom() : base() {
      quickBroom = true;
    }

    public override async Task StartState()
    {
      await base.StartState();
      particleMaker.createStars(transform.position);
    }
  }

  public class NovaDash : Broom {
    public NovaDash() : base() {
      quickBroom = true;
      targetBroomSpeed = 16;
      steerSpeed = 0;
      smoothTime = 0.25f;
      requiresNova = true;

      behaviors.Add(new Behaviors.NovaBehavior().GainSpeed(NovaManager.GainSpeed.HoldCharged));
    }

    public override async Task StartState()
    {
      await base.StartState();
      pc.novaManager.setNovaDashing(true);
      broomEffectsController.createNovaAirPuff();
    }

    public override void PostExitState()
    {
      base.PostExitState();
      pc.novaManager.setNovaDashing(false);
    }

    protected override void SetBroomEffects() {
      pc.broomEffects.SetTrail(true);
      pc.broomEffects.SetSpeedCone(true);
    }
  }
}