using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using States;
using Transitions;

[Serializable]
public abstract class AtlasState : State
{
    public PlayerController pc;
    public AtlasSpriteController spriteController;
    public BroomEffectsController broomEffectsController;

    public static readonly List<IStateTransition> defaultTransitions = 
      new List<IStateTransition> {
          new CanAttack(),
          new CanBroom(),
          new CanJump<GroundJump>(),
          new CanSlip(),
          new CanLift(),
          new CanCrouch(),
          new RunIdle(),
          new CanTurnAround(),
          new CanFall(),
      };

    protected override void attachComponents() {
        base.attachComponents();
        pc = (PlayerController)stateMachine;
        broomEffectsController = stateMachine.GetComponentInChildren<BroomEffectsController>();
        spriteController = stateMachine.GetComponentInChildren<AtlasSpriteController>();
    }
}
