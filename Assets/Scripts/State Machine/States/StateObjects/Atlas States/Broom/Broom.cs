using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using System.Linq;
using System;
using UnityEngine.Rendering.UI;
using SuperTiled2Unity.Editor.LibTessDotNet;
using MyBox;
using System.ComponentModel;
using Behaviors;
using Transitions;

namespace States {
  public class Broom : AtlasState {
    protected CharacterController cc;
    protected bool quickBroom;
    public bool requiresNova {get; protected set;}
    BroomBehavior broomBehavior;

    float upsideDownStartTime = Mathf.Infinity;
    bool upsideDown;

    public Broom() {
      Construct(null);
    }

    public Broom(BroomBehavior previousBroomBehavior) {
      Construct(previousBroomBehavior);
    }

    private void Construct(BroomBehavior previousBroomBehavior) {
      broomBehavior = previousBroomBehavior != null ? 
                previousBroomBehavior.CanWallRide(false) : 
                new BroomBehavior().TargetBroomSpeed(8).SteerSpeed(2);

      behaviors = new List<IStateBehavior>() {
        broomBehavior,
      };

      transitions = new List<IStateTransition>() {
        new Transitions.CanAttack(),
        new Transitions.CanJump<DoubleJump>(),
        new IsWallRide(),
      };
    }

    public override async Task StartState() {
      await base.StartState();
      pc = (PlayerController)stateMachine;
      cc = controller;

      pc.hasMomentum = true;
      cc.canDescendRamps = false;
      controller.canGravity = false;

      if (!skipStartAnim) {
        pc.setFacing(AtlasHelpers.Sign(AtlasInputManager.getAxis("Dpad").getValue().x));

        SoundManager.Instance.playClip("onBroom");

        controller.velocity = Vector3.zero;

        if (!quickBroom) {
          await WaitForPhaseAnimation(StateMachine.Phase.Start);
        } else {
          //await AtlasHelpers.WaitSeconds(0.1f);
        }

        SetBroomEffects();
        SoundManager.Instance.playClip("broomLaunch");
      }

      SetAnimation(StateMachine.Phase.Update);
    }

    public override void UpdateState() {
      base.UpdateState();
      FlipOverIfUpsideDown();  
      StabilizeToHorizontal();
    }

    public override async Task ExitState() {
      await base.ExitState();
      await Task.Yield();
    }

    float timeBeforeStabilize = 0.1f;
    private void StabilizeToHorizontal() {
      if (StateTime() >= timeBeforeStabilize && cc.velocity.y > 0) {
        cc.velocity.y += cc.GetGravity() * Time.deltaTime;
        if (cc.velocity.magnitude < broomBehavior.targetBroomSpeed) {
          cc.velocity += broomBehavior.targetBroomSpeed * Vector3.right * pc.facing * Time.deltaTime;
        }
        spriteController.targetZRotation = Vector2.Angle(cc.velocity.normalized, Vector2.right) + 
                                                        (90 - 90 * pc.facing);
      }
    }

    private void FlipOverIfUpsideDown() {
      float rotationAngle = Mathf.Abs(sprite.transform.localEulerAngles.z);
      upsideDown = rotationAngle >= 100 && rotationAngle <= 260;

      if (!upsideDown) upsideDownStartTime = Mathf.Infinity;
      if (upsideDown && upsideDownStartTime == Mathf.Infinity) {
        upsideDownStartTime = Time.time;
      }
      if (Time.time - upsideDownStartTime > 0.4f) {
        spriteController.SetZRotation(0);
        pc.setFacing(-pc.facing);
        anim.SetBool("BroomFlipOver", true);
      } else {
        anim.SetBool("BroomFlipOver", false);
      }
    }

    protected virtual void SetBroomEffects() {
      //pc.broomEffects.SetTrail(true);
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
      // targetBroomSpeed = 16;
      // steerSpeed = 0;
      // smoothTime = 0.25f;
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