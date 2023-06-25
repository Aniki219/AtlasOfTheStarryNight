using UnityEngine;
using System.Threading.Tasks;

public abstract class StateMachine : MonoBehaviour {
  public State state { get; private set; } = new States.Init();
  public State startState;

  public StateDisplay stateDisplay;
  public Phase phase {get; private set;} = Phase.Start;

  public int facing {get; protected set;} = 1;

  public enum Phase {
    Start,
    Update,
    Exit
  }

  public virtual void Start() {
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
  }
  ChangeStateRequest changeStateRequest;
  public void changeState(State newState, bool skipWaitForExit = false) {
    if (checkState(newState)) {
      return;
    }
    if (phase.Equals(Phase.Update)) {
      changeStateRequest = new ChangeStateRequest();
      changeStateRequest.newState = newState;
      changeStateRequest.skipWaitForExit = skipWaitForExit;  
    } else {
      doChangeState(newState, skipWaitForExit);
    }
  }

  private async void doChangeState(State newState, bool skipWaitForExit = false) {
    changePhase(Phase.Exit);
    if (skipWaitForExit) {
      _ = state.ExitState();
      await Task.Delay(16);
    } else {
      await state.ExitState();
    }
    
    state = newState;
    changePhase(Phase.Start);
    await state.StartState(this);
    changePhase(Phase.Update);
  }

  private void changePhase(Phase to) {
    phase = to;
    if (stateDisplay) {
      stateDisplay.setPhase(to.ToString());
      stateDisplay.setStateInfo(state);
    }
  }

  public bool checkState(State other) {
    return state.GetType().Equals(other.GetType());
  }

  public virtual void setFacing(float vel)
  {
    int dir = AtlasHelpers.Sign(vel);
    if (dir != 0) facing = dir;
  }
}