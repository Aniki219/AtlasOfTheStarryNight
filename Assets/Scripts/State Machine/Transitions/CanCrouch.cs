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

        Vector2 crouchColliderSize = Vector2.Scale(state.boxCollider.size, new Vector3(1.0f, 0.5f));
        Vector2 crouchColliderOffset = Vector2.up * (state.boxCollider.offset.y - state.boxCollider.size.y * 0.25f);
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

            changeState(new Move());
        }
    }
}
}