using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Behaviors {
[Serializable]
public class MoveBehavior : IStateBehavior
{
    public bool canTurnAround = true;
    float msMod = 1.0f;

    float xVelocitySmoothing;

    PlayerController pc;
    CharacterController cc;

    public override void attach(State state) {
        this.state = state;
        pc = (PlayerController)state.stateMachine;
        cc = state.controller;
    }

    public async override Task StartBehavior() {
        cc.OnBonkCeiling.AddListener(bonkCeilingListener);
        await Task.Yield();
    }

    public override void UpdateBehavior() {
        float targetVelocityX = AtlasInputManager.getAxis("Dpad").getValue().x * msMod * pc.moveSpeed;

        //if (canTurnAround) pc.setFacing(targetVelocityX);

        float smoothTime = 0;
        bool isAccelerating = AtlasHelpers.Sign(cc.velocity.x) == AtlasHelpers.Sign(targetVelocityX) && 
                              Mathf.Abs(targetVelocityX) >= Mathf.Abs(cc.velocity.x);
        if (cc.collisions.getBelow()) {
            smoothTime = isAccelerating ? pc.groundAccelerationTime : pc.groundDeccelerationTime;
        } else {
            smoothTime = isAccelerating ? pc.airAccelerationTime : pc.airDeccelerationTime;
        }
        cc.velocity.x = Mathf.SmoothDamp(cc.velocity.x, targetVelocityX, ref xVelocitySmoothing, smoothTime);
    }

    public async override Task ExitBehavior() {
        cc.OnBonkCeiling.RemoveListener(bonkCeilingListener);
        await Task.Yield();
    }

    public void bonkCeilingListener() {
        if (cc.velocity.y <= 0) return;

        state.deformer.startDeform(new Vector3(1.0f, 0.75f, 1.0f), 0.05f, 0.05f, Vector2.up);
        state.particleMaker.createStars(state.transform.position + 0.2f * Vector3.up);
        cc.velocity.y = Mathf.Min(0, cc.velocity.y/2);
        cc.resetGravity();
    }

    public MoveBehavior CanTurnAround(bool canturn = true) {
        canTurnAround = canturn;
        return this;
    }

    public MoveBehavior MoveSpeedMod(float mod) {
        msMod = mod;
        return this;
    }
}
}
