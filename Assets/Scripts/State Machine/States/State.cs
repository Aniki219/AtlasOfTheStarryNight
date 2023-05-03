using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

[Serializable]
public abstract class State
{
    public StateMachine stateMachine;
    public Transform transform;
    
    protected List<IStateBehavior> behaviors;
    protected List<IStateTransition> transitions;
    protected List<Resetters> resetters;

    public bool hasStarted {get; private set;} = false;

    public virtual async Task StartState(StateMachine stateMachine, bool wasActive = false) {
        this.stateMachine = stateMachine;
        attachComponents();

        behaviors.ForEach(b => {if (!b.waitForStart) b.StartBehavior();});
        var waitForStartBehaviors = behaviors.Where(b => b.waitForStart).Select(b => b.StartBehavior());
        await Task.WhenAll(waitForStartBehaviors);

        hasStarted = true;
    }

    public virtual void UpdateState() {
        behaviors.ForEach(b => b.UpdateBehavior());
        transitions.ForEach(t => {
            if (t.isActive) t.checkCondition();
        });
    }

    public virtual async Task ExitState() {
        behaviors.ForEach(b => {if (!b.waitForExit) b.ExitBehavior();});
        var waitForExitBehaviors = behaviors.Where(b => b.waitForExit).Select(b => b.ExitBehavior());
        await Task.WhenAll(waitForExitBehaviors);
    }

    protected void PauseTransition<T>() where T : IStateTransition {
        GetTransition<T>().pause();
    }

    protected void UnpauseTransition<T>() where T : IStateTransition {
        GetTransition<T>().unpause();
    }

    public IStateTransition GetTransition<T>() where T : IStateTransition {
        IStateTransition transition = transitions.Find(t => t.GetType().Equals(typeof(T)));
        if (transition == null) throw new Exception(GetType() + " does not contain a " + typeof(T));
        return transition;
    }

#region Components
    public Animator anim;
    public Deformer deformer;
    public Transform sprite;
    public BoxCollider2D boxCollider;
    public particleMaker particleMaker;
    public characterController controller;
    public atlasSpriteController spriteController;

    protected virtual void attachComponents() {
        transform = stateMachine.GetComponent<Transform>();
        controller = stateMachine.GetComponent<characterController>();
        boxCollider = stateMachine.GetComponent<BoxCollider2D>();
        particleMaker = stateMachine.GetComponentInChildren<particleMaker>();
        
        anim = stateMachine.GetComponentInChildren<Animator>();
        sprite = anim.transform;
        deformer = stateMachine.GetComponentInChildren<Deformer>();

        behaviors.ForEach(b => b.attach(this));
        transitions.ForEach(t => t.attach(this));
    }
#endregion
}
