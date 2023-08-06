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

    public float stateStartTime {get; private set;}

    public virtual void Attach(StateMachine stateMachine) {
        this.stateMachine = stateMachine;
        attachComponents();
    }

    public virtual async Task StartState() {
        stateStartTime = Time.time;

        SetAnimation();

        behaviors.ForEach(b => {if (!b.waitForStart) b.StartBehavior();});
        var tasks = behaviors.Where(b => b.waitForStart).Select(b => b.StartBehavior());
        await Task.WhenAll(tasks);
    }

    public virtual void UpdateState() {
        behaviors.Where(b => !b.paused)
            .ToList()
            .ForEach(b => b.UpdateBehavior());
        transitions.Where(t => t.canTransition())
            .ToList()
            .ForEach(t => t.checkCondition());
    }

    public virtual async Task ExitState() {
        behaviors.Where(b => !b.waitForExit)
            .ToList()
            .ForEach(b => b.ExitBehavior());
        var tasks = behaviors.Where(b => b.waitForExit).Select(b => b.ExitBehavior());
        await Task.WhenAll(tasks);
    }

    public virtual void PostExitState() {
        behaviors.ForEach(b => b.PostExitBehavior());
    }

    protected void PauseTransition<T>() where T : IStateTransition {
        GetTransition<T>().Pause();
    }

    protected async Task PauseTransitions(float time) {
        List<IStateTransition> ts = transitions.Where(t => !t.paused).Select(t => t.Pause()).ToList();
        await AtlasHelpers.WaitSeconds(time);
        ts.ForEach(t => t.Unpause());
    }

    protected async Task PauseTransitions(float time, params IStateTransition[] trans) {
        List<IStateTransition> ts = trans.Where(t => !t.paused).Select(t => t.Pause()).ToList();
        await AtlasHelpers.WaitSeconds(time);
        ts.ForEach(t => t.Unpause());
    }

    protected void PauseTransitions() {
        transitions.ForEach(t => t.Pause());
    }

    protected void UnpauseTransitions() {
        transitions.ForEach(t => t.Unpause());
    }

    protected async void PauseTransition<T>(float seconds) where T : IStateTransition {
        IStateTransition t = GetTransition<T>();
        t.Pause();
        await AtlasHelpers.WaitSeconds(seconds);
        t.Unpause();
    }

    protected void UnpauseTransition<T>() where T : IStateTransition {
        GetTransition<T>().Unpause();
    }

    public void PauseBehavior<T>() where T : IStateBehavior {
        GetBehavior<T>().Pause();
    }

    public async void PauseBehavior<T>(float seconds) where T : IStateBehavior {
        IStateBehavior b = GetBehavior<T>();
        b.Pause();
        await AtlasHelpers.WaitSeconds(seconds);
        b.Unpause();
    }

    public void UnpauseBehavior<T>() where T : IStateBehavior {
        GetBehavior<T>().Unpause();
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

    public async Task WaitForPhaseAnimation(StateMachine.Phase phase) {
        try {
        await AtlasHelpers.WaitSeconds(
            Mathf.Max(FindStatePhaseClip(phase).length - stateTime(), 0)
        );
        } catch(Exception e) {
            Debug.Log(GetType().Name + phase.ToString() + e);
        }
    }

    public float stateTime() {
        return Time.time - stateStartTime;
    }

    public void SetAnimation(StateMachine.Phase phase = StateMachine.Phase.Start) {
        if (!anim) return;
        int stateHash = FindStatePhaseHash(phase);
        if (stateHash != -1) anim.Play(stateHash);
    }

    public AnimationClip FindStatePhaseClip(StateMachine.Phase phase, bool canCheckGenericName = true) {
        string stateName = GetType().Name;
        string phaseName = phase.Equals(StateMachine.Phase.Update) ? "" : phase.ToString();
        AnimationClip clip = AtlasHelpers.FindAnimation(anim, stateName + phaseName);
        if (clip == null && canCheckGenericName) clip = AtlasHelpers.FindAnimation(anim, stateName);
        return clip;
    }

    public int FindStatePhaseHash(StateMachine.Phase phase, bool canCheckGenericName = true) {
        string stateName = GetType().Name;
        string phaseName = phase.Equals(StateMachine.Phase.Update) ? "" : phase.ToString();
        if (anim.HasState(0, Animator.StringToHash(stateName + phaseName))) {
            return Animator.StringToHash(stateName + phaseName);
        }
        if (canCheckGenericName && anim.HasState(0, Animator.StringToHash(stateName))) {
            return Animator.StringToHash(stateName);
        }
        return -1;
    }

    public State RemoveTransition<T>() where T : IStateTransition {
        transitions.Remove(GetTransition<T>());
        return this;
    }

    public virtual void OnAnimationEnd() {}

#region Components
    public Animator anim;
    public Deformer deformer;
    public Transform sprite;
    public ColliderManager colliderManager;
    public ParticleMaker particleMaker;
    public characterController controller;
    public atlasSpriteController spriteController;
    public BroomEffectsController broomEffectsController;

    protected virtual void attachComponents() {
        transform = stateMachine.GetComponent<Transform>();
        controller = stateMachine.GetComponent<characterController>();
        colliderManager = stateMachine.GetComponent<ColliderManager>();
        particleMaker = stateMachine.GetComponentInChildren<ParticleMaker>();
        broomEffectsController = stateMachine.GetComponentInChildren<BroomEffectsController>();
        
        anim = stateMachine.GetComponentInChildren<Animator>();
        sprite = anim.transform;
        spriteController = stateMachine.GetComponentInChildren<atlasSpriteController>();
        deformer = stateMachine.GetComponentInChildren<Deformer>();

        behaviors.ForEach(b => b.attach(this));
        transitions.ForEach(t => t.attach(this));
    }
#endregion
}
