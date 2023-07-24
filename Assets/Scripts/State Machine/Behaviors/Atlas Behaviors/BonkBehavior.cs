using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Behaviors {
[Serializable]
public class BonkBehavior : IStateBehavior
{
    public bool reset;
    public int damage;

    PlayerController pc;
    bool hasStarted = false;

    public BonkBehavior() {
        waitForStart = false;
    }
    
    public async override Task StartBehavior() {
        pc = (PlayerController)state.stateMachine;

        if (!state.controller.collisions.isTangible()) { return; }
        state.controller.collisions.setTangible(false);

        if (reset)
        {
            pc.resetPosition = true;
            SoundManager.Instance.playClip("hurt2");
        } else { 
            if (damage > 0)
            {
                SoundManager.Instance.playClip("hurt");
                pc.setInvulnerable(2.0f);
            } else
            {
                SoundManager.Instance.playClip("bonk");
            }
            pc.createStars(state.transform.position);
        }

        pc.resetAnimator();
        state.deformer.startDeform(new Vector3(.25f, 1.1f, 1), 0.1f, .35f, Vector2.right * pc.facing);
        
        state.controller.lockPosition = true;
        await AtlasHelpers.WaitSeconds(0.1f);
        state.controller.lockPosition = false;
        

        if (damage > 0) { ResourceManager.Instance.takeDamage(damage); }

        state.controller.velocity.y = 4f;
        state.controller.velocity.x = -pc.moveSpeed/4f * pc.facing;

        AtlasEventManager.Instance.BonkEvent();
        
        Camera.main.GetComponent<cameraController>().StartShake();
    }

    public override void UpdateBehavior() {
        if (!pc.isGrounded())
        {
            state.controller.velocity.x = -pc.moveSpeed/4f * pc.facing;
        }
    }

    public async override Task ExitBehavior() {
        await Task.Yield();
    }
}
}
