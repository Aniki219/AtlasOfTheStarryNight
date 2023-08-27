using UnityEngine;
using System;
using System.Threading.Tasks;

public abstract class StateMachine : MonoBehaviour {
  public State state { get; private set; } = new States.Init();
  public State startState;

//TODO: No dude
  public StateDisplay stateDisplay;
  public Phase phase {get; protected set;} = Phase.Start;

  public enum Phase {
    Start,
    Update,
    Exit
  }

  public virtual void Start() {
    startState.Attach(this);
    changeState(startState);
  }

  public virtual void Update() {
    if (!phase.Equals(Phase.Update) && !state.alwaysUpdate) return;
    if (gameManager.isPaused) return;
    state.UpdateState();

    if (changeStateRequest.newState != null) {
      doChangeState(changeStateRequest.newState, changeStateRequest.skipWaitForExit);
      changeStateRequest = default;
    }
  }

  struct ChangeStateRequest {
    public State newState;
    public bool skipWaitForExit;

    public ChangeStateRequest(State _newState, bool _skipWaitForExit) {
      newState = _newState;
      skipWaitForExit = _skipWaitForExit;
    }
  }

  ChangeStateRequest changeStateRequest;
  public void changeState(State newState, bool skipWaitForExit = false) {
    if (checkState(newState)) {
      return;
    }
    if (phase.Equals(Phase.Update)) {
      changeStateRequest = new ChangeStateRequest(newState, skipWaitForExit);
    } else {
      doChangeState(newState, skipWaitForExit);
    }
  }

  public void changeState<T>(bool skipWaitForExit = false) where T : State {
    changeState((T) Activator.CreateInstance(typeof(T)));
  }

  /* Exit finishes and Start begins on the same Update tick
  right now I want to use this fact for linking together novadash behavior
  */
  private async void doChangeState(State newState, bool skipWaitForExit = false) {
    changePhase(Phase.Exit);
    if (skipWaitForExit) {
      _ = state.ExitState();
      await Task.Delay(16);
    } else {
      await state.ExitState();
    }
    state.PostExitState();
    
    state = newState;
    newState.Attach(this);
    changePhase(Phase.Start);
    await state.StartState();
    changePhase(Phase.Update);
  }

  protected virtual void changePhase(Phase to) {
    phase = to;
    if (stateDisplay) {
      stateDisplay.setPhase(to.ToString());
      stateDisplay.setStateInfo(state);
    }
  }

  public bool checkState(State other) {
    return state.GetType().Equals(other.GetType());
  }
}