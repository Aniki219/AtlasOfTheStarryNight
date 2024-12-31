using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Behaviors {
[Serializable]
public class AttackBehavior : IStateBehavior
{
    bool arialAttacking;
    public bool resetVelocity;
    public Vector2 stepVelocity;

    public async override Task StartBehavior() {
        //TODO: See if we can move `selectAttack()` into here
        state.deformer.RemoveDeform("fastfall");
        if (resetVelocity) state.controller.velocity = Vector3.zero;
        if (!state.controller.IsGrounded())
        {
            arialAttacking = true;
        }
        await Task.Yield();
    }

    public override void UpdateBehavior() {
        state.anim.SetFloat("animTime", state.anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    public async override Task ExitBehavior() {
        arialAttacking = false;
        await Task.Yield();
    }

    public AttackBehavior ResetVelocity() {
        resetVelocity = true;
        return this;
    }
    
    public AttackBehavior StepVelocity(float vel) {
        stepVelocity = Vector2.right * state.stateMachine.facing * vel;
        return this;
    }
}
}