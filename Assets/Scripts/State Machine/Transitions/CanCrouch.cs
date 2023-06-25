using UnityEngine;
using System;
using States;

namespace Transitions {
public class CanCrouch : IStateTransition {
    playerController pc;
    characterController cc;

    public override void attach(State state) {
        base.attach(state);
        pc = (playerController)state.stateMachine;
        cc = state.controller;    
    }

    public override void checkCondition() {
        if (AtlasInputManager.getAxisState("Dpad").y < 0)
        {
            changeState(new Crouch());
            return;
        }

        BoxCollider2D crouchCollider = state.colliderManager.getCollider("Crouching");
        Vector2 crouchColliderSize = crouchCollider.size;
        Vector2 crouchColliderOffset = crouchCollider.offset;
        if (cc.isGrounded() &&
            Mathf.Abs(cc.velocity.x) >= (pc.moveSpeed - 0.5f) &&
            AtlasHelpers.Sign(cc.collisions.getAverageNorm().x) +
                AtlasHelpers.Sign(cc.velocity.x) == 0 &&
            cc.velocityOut.sqrMagnitude < 0.0001f
        ) {
            if (Physics2D.OverlapBox(
                (Vector2)(state.transform.position + cc.velocity * Time.deltaTime) + crouchColliderOffset, 
                Vector2.Scale(crouchColliderSize, (Vector2)state.transform.localScale), 
                0, 
                cc.collisionMask
            ) == null)
            changeState(new Crouch());
        }    
    }
}

public class CanUncrouch : IStateTransition {
    public override void checkCondition()
    {
        if (AtlasInputManager.getAxisState("Dpad").y >= 0 && state.controller.checkVertDist(0.3f)) {
            changeState(new Idle());
        }
    }
}
}