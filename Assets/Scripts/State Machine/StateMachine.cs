using UnityEngine;
using System.Threading.Tasks;

public abstract class StateMachine : MonoBehaviour {
  public State state { get; private set; }
  public State startState;

  public StateDisplay stateDisplay;
  public Phase phase {get; private set;} = Phase.Start;

  public enum Phase {
    Start,
    Update,
    Exit
  }

  public virtual void Start() {
    changeState(startState);
  }

  public virtual void Update() {
    if (!phase.Equals(Phase.Update)) return;
    if (gameManager.isPaused) return;
    state.UpdateState();
  }

  public async void changeState(State newState, bool skipWaitForExit = false) {
    bool sameState = false; //checkState(newState);

    if (state != null) {
      changePhase(Phase.Exit);
      if (skipWaitForExit) {
        state.ExitState();
      } else {
        await state.ExitState();
      }
    }
    state = newState;
    changePhase(Phase.Start);
    await state.StartState(this, sameState);
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
}