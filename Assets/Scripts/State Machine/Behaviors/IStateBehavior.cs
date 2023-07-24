using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[Serializable]
public abstract class IStateBehavior
{
    protected State state;
    public bool waitForStart {get; protected set;} = true;
    public bool waitForExit {get; protected set;} = true;
    public bool paused {get; protected set;} = false;

    public virtual void attach(State state) {
        this.state = state;
    }

    public abstract Task StartBehavior();

    public abstract void UpdateBehavior();

    public abstract Task ExitBehavior();

    public virtual void PostExitBehavior() {}

    public void Pause() {
        paused = true;
    }

    public void Unpause() {
        paused = false;
    }
}

