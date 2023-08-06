﻿using UnityEngine;
using System;

public abstract class IStateTransition {
    protected State state;
    public bool paused {get; protected set;} = false;
    public bool skipWaitForExit {get; protected set;} = false;
    public float transitionTime {get; protected set;}

    public virtual void attach(State state) {
        this.state = state;
    }

    public virtual void changeState(State newState) {
        state.stateMachine.changeState(newState, skipWaitForExit);
    }

    public virtual void checkCondition() {}



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

    public bool canTransition() {
        return !paused && (transitionTime < state.stateTime());
    }

    public IStateTransition TransitionTime(float time) {
        transitionTime = time;
        return this;
    }

    public State getNewState<T>() where T : State {
        return (T) Activator.CreateInstance(typeof(T));
    }
}