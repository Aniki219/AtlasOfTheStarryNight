using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using States;

namespace Behaviors {
    [Serializable]
    public class BroomBehavior : IStateBehavior
    {
      PlayerController pc;
      CharacterController cc;
      AtlasState astate;

      public float targetBroomSpeed {get; private set;}
      public float currentBroomSpeed {get; private set;}
      public float steerSpeed {get; private set;}
      private Vector3 steerVector;
      float smoothTime = 0;

      private bool canWallRide;

      public BroomBehavior TargetBroomSpeed(float tbs) {
        targetBroomSpeed = tbs;
        return this;
      }

      public BroomBehavior SteerSpeed(float ss) {
        steerSpeed = ss;
        return this;
      }

      public async override Task StartBehavior() {
        astate = (AtlasState)state;
        pc = (PlayerController)state.stateMachine;
        cc = state.controller;
        await Task.Yield();
      }

      public override void UpdateBehavior() {
        if (CancelBroom()) {
          pc.ChangeState(new Fall());
          return;
        }

        if (cc.CollisionInMoveDirection()) {
          if (cc.HasSteepCollision()) {         
            pc.StartBonk();
          } else {
            DoWallRide();
          }
          return;
        }

        SteerBroom();
        SetBroomSpeed();
        cc.Move(steerVector);
      }

      private void SetBroomSpeed() {
        if (smoothTime == 0) {
          currentBroomSpeed = targetBroomSpeed;
        } else {
          currentBroomSpeed = EasingFunctions.EaseInQuint(4, targetBroomSpeed, Mathf.Clamp(state.StateTime()/smoothTime, 0, 1));
        }
        Vector3 velocityNorm = (cc.velocity.normalized == Vector3.zero) ? pc.facing * Vector3.right : cc.velocity.normalized;
        cc.velocity = velocityNorm * currentBroomSpeed;
      }

      private void DoWallRide() {
        if (!canWallRide) return;
        Vector3 n = cc.collisions.getAverageNorm();
        Vector3 d = cc.velocity - Vector3.Dot(cc.velocity, n) * n;

        float dot = Vector2.Dot(AtlasInputManager.getAxis("Dpad").getValue(), n);
        bool pressIntoWall = dot <= -0.5f;

        

        cc.velocity = targetBroomSpeed * d.normalized;

        float dotp = Vector3.Dot(n, Vector3.right);
        float theta = (1 - dotp) * ((n.y >= 0) ? 90 : -90);

        astate.spriteController.targetZRotation = theta - 90;

        pc.setFacing(-Vector3.Cross(n, cc.velocity.x*Vector3.right).z);
      }

      private bool CancelBroom() {
        if (state.StateTime() > 0.1f) {
          if (AtlasInputManager.getKeyPressed("Broom") || 
          (!AtlasInputManager.getKey("Broom") && AtlasInputManager.Instance.holdBroom))
          {
            return true;
          }
        }
        return false;
      }

      private void SteerBroom() {
        float vdir = AtlasInputManager.getAxis("Dpad").pressedAfter(state.stateStartTime) ? 
              AtlasInputManager.getAxis("Dpad").getValue().y : 0;
        steerVector = vdir * steerSpeed * Vector3.up;
      }

      public BroomBehavior CanWallRide(bool can = true) {
        canWallRide = can;
        return this;
      }

      public override async Task ExitBehavior() {
        cc.canDescendRamps = true;

        pc.broomEffects.SetTrail(false);
        pc.broomEffects.SetSpeedCone(false);
        cc.canGravity = true;
        cc.momentum = cc.velocity;
        cc.initialMomentum = cc.velocity - cc.velocity.normalized * 2;
        cc.momentumStartTime = Time.time;
        cc.momentumHangTime = 0;
        cc.momentumDecayTime = 0.5f;
        cc.resetGravity();
        cc.velocity = Vector2.zero;
        AtlasEventManager.Instance.BroomCancelEvent();

        astate.spriteController.targetZRotation = 0;
        await Task.Yield();
      }
    }
}