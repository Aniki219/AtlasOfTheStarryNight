﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using UnityEngine.SceneManagement;

namespace Behaviors {
[Serializable]
public class CarryBehavior : IStateBehavior
{
    PlayerController pc;
    Transform heldObject;

    public async override Task StartBehavior() {
        pc = (PlayerController)state.stateMachine;
        heldObject = pc.heldObject;
        await Task.Yield();
    }

    public override void UpdateBehavior() {
        if (!state.anim.GetBool("isHolding") ||
            !heldObject) {
                state.stateMachine.ChangeState(new States.Idle());
        }

        heldObject.transform.localScale = new Vector3(pc.facing, 1, 1);
        if (AtlasInputManager.getKeyPressed("Down"))
        {
            dropHolding().Start();
        } 
        if (AtlasInputManager.getKeyPressed("Up")) {
            throwHolding().Start();
        }
    }

    async Task throwHolding()
    {
        await dropHolding(true);
    }

    async Task dropHolding(bool throwing = false)
    {
        if (!pc.heldObject) return;
        Rigidbody2D rb = heldObject.GetComponent<Rigidbody2D>();
        rb.simulated = true;

        if (throwing)
        {
            await AtlasHelpers.WaitSeconds(0.4f);
            rb.AddForce(new Vector2(200.0f * pc.facing, 75.0f));
        } else
        {
            rb.AddForce(new Vector2(0, 50.0f));
        }

        heldObject.parent = null;
        SceneManager.MoveGameObjectToScene(heldObject.gameObject, SceneManager.GetActiveScene());
        heldObject = null;
        state.stateMachine.ChangeState(new States.Idle());
    }

    public async override Task ExitBehavior() {
        pc.heldObject = null;
        pc.ChangeState(new States.Idle());
        await Task.Yield();
    }
}

}