using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public abstract class StateMachine : MonoBehaviour {
  public State state { get; private set; }
  public State startState;

  public TextMeshProUGUI stateDisplay;

  public virtual void Start() {
    changeState(startState);
  }

  public virtual void Update() {
    if (!state.hasStarted) return;
    state.UpdateState();
  }

  public async void changeState(State newState) {

    bool sameState = false; //checkState(newState);
  
    if (state != null) await state.ExitState();
    state = newState;
    await state.StartState(this, sameState);

    if (stateDisplay) stateDisplay.text = state.GetType().ToString();
  }

  public bool checkState(State other) {
    return state.GetType().Equals(other.GetType());
  }
}