using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityAsync;

namespace Behaviors {
[Serializable]
public class JumpBehavior : IStateBehavior
{
    public Vector2 velocity;
    public float apexTime;
    public float apexHeight;
    public int lockOutTime;
    public bool stopVelocityOnEntry;
    public bool variableJump;

    private playerController pc;
    private characterController cc;

    public JumpBehavior(Vector2 velocity, bool variableJump = false, int lockOutTime = 0) {
        this.velocity = velocity;
        this.variableJump = variableJump;
        this.lockOutTime = lockOutTime;
        waitForStart = false;
    }

    public override void attach(State state) {
        base.attach(state);
        pc = (playerController)state.stateMachine;
        cc = state.controller;
    }

    public async override Task StartBehavior() {
        state.anim.SetBool("isJumping", true);
        float startTime = Time.time;

        int maxFrames = 6;

        cc.velocity.y = velocity.y;
        cc.velocity.x += velocity.x * ((playerController)state.stateMachine).facing;
        
        cc.resetGravity();

        while (variableJump && maxFrames > 0) {
            if (!AtlasInputManager.getKey("Jump")) {
                cc.velocity.y /= 4;
                break;
            }
            await AtlasHelpers.WaitSeconds(4 / 60.0f);
            maxFrames--;
        }

        await AtlasHelpers.WaitSeconds(lockOutTime);
    }

    public override void UpdateBehavior() {
        
    }

    public async override Task ExitBehavior() {
        state.anim.SetBool("isJumping", false);
        await Task.Yield();
    }
}
}
