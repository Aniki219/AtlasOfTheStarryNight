using UnityEngine;
using System;

public abstract class IStateTransition {
    protected State state;
    public bool isActive {get; protected set;} = true;
    public bool skipWaitForExit {get; protected set;} = false;

    public virtual void attach(State state) {
        this.state = state;
    }

    public virtual void changeState(State newState, bool skipWaitForExit = false) {
        state.stateMachine.changeState(newState, skipWaitForExit);
    }

    public abstract void checkCondition();

    
    public void pause() {
        isActive = false;
    }

    public void unpause() {
        isActive = true;
    }
}
