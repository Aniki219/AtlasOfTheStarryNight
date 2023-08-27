using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

[Serializable]
public abstract class State
{
    //TODO: We need to make EntityState class
    public EntityStateMachine stateMachine;
    public Transform transform;
    
    public List<IStateBehavior> behaviors {get; protected set;} = new List<IStateBehavior>();
    public List<IStateTransition> transitions {get; protected set;} = new List<IStateTransition>();
    protected List<Resetters> resetters;

    public bool hasStarted {get; private set;} = false;
    public bool hasEnded {get; private set;} = false;

    protected bool skipStartAnim = false;
    public bool alwaysUpdate = false;

    public float stateStartTime {get; private set;}

    float clipLength = Mathf.Infinity;

    public virtual void Attach(StateMachine stateMachine) {
        //TODO EntityState
        this.stateMachine = (EntityStateMachine)stateMachine;
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
        if (StateTime() >= clipLength) OnAnimationEnd();
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
            Mathf.Max(FindStatePhaseClip(phase).length - StateTime(), 0)
        );
        } catch(Exception e) {
            Debug.Log(GetType().Name + phase.ToString() + e);
        }
    }

    public float StateTime() {
        return Time.time - stateStartTime;
    }

    public void SetAnimation(StateMachine.Phase phase = StateMachine.Phase.Start) {
        if (!anim) {Debug.Log(stateMachine.transform.name + " has no animator"); return;}
        int stateHash = FindStatePhaseHash(phase);
        if (stateHash != -1) anim.Play(stateHash);

        AnimationClip clip = FindStatePhaseClip(StateMachine.Phase.Start);
        if (clip) {
            clipLength = clip.length;
        } else {
            Debug.LogWarning("No clip found for " + GetType());
        }
    }

    public AnimationClip FindStatePhaseClip(StateMachine.Phase phase, bool canCheckGenericName = true) {
        string stateName = stateMachine.animClipPrefix + GetType().Name;
        string phaseName = phase.Equals(StateMachine.Phase.Update) ? "" : phase.ToString();

        AnimationClip clip = AtlasHelpers.FindAnimation(anim, stateName + phaseName);
        if (clip == null && canCheckGenericName) clip = AtlasHelpers.FindAnimation(anim, stateName);
        return clip;
    }

    public int FindStatePhaseHash(StateMachine.Phase phase, bool canCheckGenericName = true) {
        string stateName = stateMachine.animClipPrefix + GetType().Name;
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
    public CharacterController controller;

    protected virtual void attachComponents() {
        transform = stateMachine.GetComponent<Transform>();
        controller = stateMachine.GetComponent<CharacterController>();
        colliderManager = stateMachine.GetComponent<ColliderManager>();
        particleMaker = stateMachine.GetComponentInChildren<ParticleMaker>();
                
        anim = stateMachine.GetComponentInChildren<Animator>();
        sprite = anim.transform;
        deformer = stateMachine.GetComponentInChildren<Deformer>();

        behaviors.ForEach(b => b.attach(this));
        transitions.ForEach(t => t.Attach(this));
    }
#endregion

    public class TPManager
    {
        private readonly List<TransitionProbability> transitionProbabilities;
        private readonly State state;
        
        public TPManager(State _state, params TransitionProbability[] tps) {
            state = _state;
            transitionProbabilities = tps.ToList();
            state.transitions.AddRange(tps.Select(tp => tp.transition));

            pauseTransitions();

            float totalAssignedProbability = transitionProbabilities.Aggregate(0f, (total, t) => total + t.probability);
            if (totalAssignedProbability > 1.0f) throw new Exception(state.GetType() + " transition probabilities exceed 1.0");
            List<TransitionProbability> unassignedProbabilities = transitionProbabilities.Where(tp => tp.probability == 0).ToList();
            
            if (unassignedProbabilities.Count() > 0) {
                float defaultProbability = 1.0f - totalAssignedProbability / unassignedProbabilities.Count();
                unassignedProbabilities.Select(tp => tp.probability = defaultProbability);
            }
        }

        public class TransitionProbability {
            public IStateTransition transition;
            public float probability;

            public TransitionProbability(IStateTransition _transition, float _probability = 0) {
                transition = _transition;
                probability = _probability;
            }
        }

        public void Update() {
            float r = UnityEngine.Random.value;

            pauseTransitions();
            float total = 0;
            foreach (TransitionProbability tp in transitionProbabilities) {
                if (tp.probability + total > r) {
                    tp.transition.Unpause();
                    break;
                }
                total += tp.probability;
            }
        }

        private void pauseTransitions() {
            transitionProbabilities.ForEach(tp => tp.transition.Pause());
        }
    }
}
