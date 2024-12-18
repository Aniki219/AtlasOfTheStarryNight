﻿using UnityEngine;
using System;
using States;

namespace Transitions {
public class CanCrouch : IStateTransition {
    PlayerController pc;
    CharacterController cc;

    public override void Attach(State state) {
        base.Attach(state);
        pc = (PlayerController)state.stateMachine;
        cc = state.controller;    
    }

    public override void checkCondition() {
        if (AtlasInputManager.getAxis("Dpad").getValue().y < 0)
        {
            changeState(new Crouch());
            return;
        }

        BoxCollider2D crouchCollider = state.colliderManager.getCollider("Crouching");
        Vector2 crouchColliderSize = crouchCollider.size;
        Vector2 crouchColliderOffset = crouchCollider.offset;
        if (cc.IsGrounded() &&
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
        if (IsAllowed(state)) {
            if (AtlasInputManager.getAxis("Dpad").getDirection().x == TiltDirection.Neutral) {
                changeState(new Idle());
            } else {
                changeState(new Run());
            }
        }
    }

    public static bool IsAllowed(State state) {
        return AtlasInputManager.getAxis("Dpad").getValue().y >= 0 && state.controller.CheckVertDist(0.3f);
    }
}
}