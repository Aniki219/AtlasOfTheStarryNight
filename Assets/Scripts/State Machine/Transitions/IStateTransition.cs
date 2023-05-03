using UnityEngine;
using System;

public abstract class IStateTransition {
    protected State state;
    public bool isActive {get; private set;} = true;

    public virtual void attach(State state) {
        this.state = state;
    }

    public virtual void changeState(State newState) {
        state.stateMachine.changeState(newState);
    }

    public abstract void checkCondition();

    
    public void pause() {
        isActive = false;
    }

    public void unpause() {
        isActive = true;
    }
}
