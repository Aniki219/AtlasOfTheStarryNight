using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Behaviors {
[Serializable]
public class BroomBehavior : IStateBehavior
{
    public int dir = 0;
    public bool novaBurst;
    public AnimationClip broomStartClip;

    playerController pc;
    characterController cc;

    private bool onBroom = false;

    public async override Task StartBehavior() {
        pc = (playerController)state.stateMachine;
        cc = state.controller;

        if (dir == 0) {
            dir = (int)AtlasHelpers.Sign(AtlasInputManager.getAxisState("Dpad").x);
        }
        pc.setFacing(dir);

        state.anim.SetTrigger("broomStart");
        SoundManager.Instance.playClip("onBroom");
        state.deformer.RemoveDeform("fastfall");
        state.deformer.RemoveDeform("jump");

        state.controller.velocity = Vector3.zero;
        state.controller.canGravity = false;

        await AtlasHelpers.WaitSeconds(AnimMapper.getClip<States.Broom>().length);

        pc.resetAnimator();
        SoundManager.Instance.playClip("broomLaunch");
        onBroom = true;
    }

    public override void UpdateBehavior() {
        if (AtlasInputManager.getKeyPressed("Broom") || 
            AtlasInputManager.getKeyPressed("Down") ||
            (!AtlasInputManager.getKey("Broom") && 
            AtlasInputManager.Instance.holdBroom))
        {
            state.stateMachine.changeState(new States.Move());
            return;
        }
        if (!onBroom) return;
        if ((pc.facing == -1) ? cc.collisions.getLeft() : cc.collisions.getRight()) {
            if (cc.collisions.hasNormWhere(norm => Mathf.Abs(Vector2.Dot(norm, Vector2.right)) > 0.8f) ||
                Mathf.Abs(Vector2.Dot(cc.collisions.getAverageNorm(), Vector2.right)) > 0.8f) {
                    pc.startBonk();
                    state.stateMachine.changeState(new States.Bonk());
                }
        }
        float vdir = AtlasInputManager.getAxisState("Dpad").y;
        cc.velocity.y = pc.moveSpeed / 2.0f * vdir;
        cc.velocity.x = pc.moveSpeed * 2 * pc.facing;
    }

    public async override Task ExitBehavior() {
        state.controller.canGravity = true;
        state.anim.SetTrigger("broomEnd");

        AtlasEventManager.Instance.BroomCancelEvent();

        await Task.Yield();
    }
}
}
