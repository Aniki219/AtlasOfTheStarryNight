using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

[Serializable]
public abstract class AtlasState : State
{
    public PlayerController pc;
    public AtlasSpriteController spriteController;
    public BroomEffectsController broomEffectsController;

    protected override void attachComponents() {
        base.attachComponents();
        pc = (PlayerController)stateMachine;
        broomEffectsController = stateMachine.GetComponentInChildren<BroomEffectsController>();
        spriteController = stateMachine.GetComponentInChildren<AtlasSpriteController>();
    }
}
