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
    private Vector2 velocity;
    private float apexHeight;
    private float jumpDistance;

    private LockoutType lockoutType = LockoutType.None;
    private float lockOutTime;
    private float startTime;

    private bool stopVelocityOnEntry;
    private bool variableJump;

    private playerController pc;
    private characterController cc;

    public enum LockoutType {
        None,
        Apex,
        TwiceApex,
        TimeSeconds
    }

    public JumpBehavior() {
        lockoutType = LockoutType.None;
        waitForStart = false;
    }

    public JumpBehavior VariableJump() {
        variableJump = true;
        return this;
    }

    public JumpBehavior JumpHeight(float tiles) {
        apexHeight = tiles/2.0f;
        return this;
    }

    public JumpBehavior JumpDistance(float tiles) {
        jumpDistance = tiles/2.0f;
        return this;
    }

    public JumpBehavior Lockout(LockoutType type, float seconds = 0) {
        lockoutType = type;
        if (lockoutType.Equals(LockoutType.TimeSeconds)) {
            lockOutTime = seconds;
        }
        if (!lockoutType.Equals(LockoutType.None)) {
            waitForStart = true;
        }
        return this;
    }

    public override void attach(State state) {
        base.attach(state);
        pc = (playerController)state.stateMachine;
        cc = state.controller;
    }

    public async override Task StartBehavior() {
        float h = apexHeight;
        float g = state.controller.gravity * state.controller.gravityMod;
        float viy = Mathf.Sqrt(-2*h*g);
        float apexTime = viy/-g;
        float vix = jumpDistance/(2*apexTime);

        state.anim.SetBool("isJumping", true);
        state.anim.SetBool("isGrounded", false);
        int maxFrames = 6;

        cc.velocity.y = Mathf.Sqrt(-2*h*g);
        if (vix > 0) cc.setXVelocity(vix);
        
        cc.resetGravity();

        switch(lockoutType) {
            case LockoutType.Apex:
                lockOutTime = apexTime;
                break;
            case LockoutType.TwiceApex:
                lockOutTime = 2*apexTime;
                break;
        }

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
