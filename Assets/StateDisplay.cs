using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class StateDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stateName;
    string statePhase = "";
    [SerializeField] TextMeshProUGUI behaviors;
    [SerializeField] TextMeshProUGUI transitions;

    public void setStateInfo(State state) {
        stateName.text = state.GetType().Name + ": " + statePhase;

        behaviors.text = string.Concat(state.behaviors.Select(b => b.GetType().Name + "\n"));    
        transitions.text = string.Concat(state.transitions.Select(t => t.GetType().Name + "\n"));
    }

    public void setPhase(string phase) {
        statePhase = phase;
    }
}
