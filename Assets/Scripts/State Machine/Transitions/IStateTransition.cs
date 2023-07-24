using UnityEngine;
using System;

public abstract class IStateTransition {
    protected State state;
    public bool paused {get; protected set;} = false;
    public bool skipWaitForExit {get; protected set;} = false;

    public virtual void attach(State state) {
        this.state = state;
    }

    public virtual void changeState(State newState) {
        state.stateMachine.changeState(newState, skipWaitForExit);
    }

    public abstract void checkCondition();



    public IStateTransition SkipWaitForExit() {
        skipWaitForExit = true;
        return this;
    }

    public IStateTransition Pause() {
        paused = true;
        return this;
    }

    public void Unpause() {
        paused = false;
    }

    public State getNewState<T>() where T : State {
        return (T) Activator.CreateInstance(typeof(T));
    }
}
