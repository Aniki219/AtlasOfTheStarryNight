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
    
    public List<IStateBehavior> behaviors {get; protected set;} = new List<IStateBehavior>();
    public List<IStateTransition> transitions {get; protected set;} = new List<IStateTransition>();
    protected List<Resetters> resetters;

    public bool hasStarted {get; private set;} = false;
    public bool hasEnded {get; private set;} = false;

    protected bool skipStartAnim = false;
    public bool alwaysUpdate = false;

    public virtual async Task StartState(StateMachine stateMachine) {
        this.stateMachine = stateMachine;
        attachComponents();

        behaviors.ForEach(b => {if (!b.waitForStart) b.StartBehavior();});
        var tasks = behaviors.Where(b => b.waitForStart).Select(b => b.StartBehavior());
        await Task.WhenAll(tasks);
    }

    public virtual void UpdateState() {
        behaviors.ForEach(b => b.UpdateBehavior());
        transitions.ForEach(t => {
            if (t.isActive) t.checkCondition();
        });
    }

    public virtual async Task ExitState() {
        behaviors.ForEach(b => {if (!b.waitForExit) b.ExitBehavior();});
        var tasks = behaviors.Where(b => b.waitForExit).Select(b => b.ExitBehavior());
        await Task.WhenAll(tasks);
    }

    protected void PauseTransition<T>() where T : IStateTransition {
        GetTransition<T>().pause();
    }

    protected async void PauseTransition<T>(float seconds) where T : IStateTransition {
        IStateTransition t = GetTransition<T>();
        t.pause();
        await AtlasHelpers.WaitSeconds(seconds);
        t.unpause();
    }

    protected void UnpauseTransition<T>() where T : IStateTransition {
        GetTransition<T>().unpause();
    }

    public T GetBehavior<T>() where T : IStateBehavior {
        IStateBehavior behavior = behaviors.Find(t => t.GetType().Equals(typeof(T)));
        if (behavior == null) throw new Exception(GetType() + " does not contain a " + typeof(T));
        return (T)behavior;
    }

    public T GetTransition<T>() where T : IStateTransition {
        IStateTransition transition = transitions.Find(t => t.GetType().Equals(typeof(T)));
        if (transition == null) throw new Exception(GetType() + " does not contain a " + typeof(T));
        return (T)transition;
    }

    public State SkipStartAnim() {
        skipStartAnim = true;
        return this;
    }

#region Components
    public Animator anim;
    public Deformer deformer;
    public Transform sprite;
    public ColliderManager colliderManager;
    public particleMaker particleMaker;
    public characterController controller;
    public atlasSpriteController spriteController;

    protected virtual void attachComponents() {
        transform = stateMachine.GetComponent<Transform>();
        controller = stateMachine.GetComponent<characterController>();
        colliderManager = stateMachine.GetComponent<ColliderManager>();
        particleMaker = stateMachine.GetComponentInChildren<particleMaker>();
        
        anim = stateMachine.GetComponentInChildren<Animator>();
        sprite = anim.transform;
        spriteController = stateMachine.GetComponentInChildren<atlasSpriteController>();
        deformer = stateMachine.GetComponentInChildren<Deformer>();

        behaviors.ForEach(b => b.attach(this));
        transitions.ForEach(t => t.attach(this));
    }
#endregion
}
