using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateObject : ScriptableObject
{
    public List<IStateBehavior> stateBehaviors;
    public List<Allowances> allowances;
    public List<Resetters> resetters;

    void Reset() {
        stateBehaviors = new List<IStateBehavior>();
        
        allowances = new  List<Allowances>() {
            Allowances.HURT,
            Allowances.GRAVITY
        };

        resetters = new List<Resetters>() {
            Resetters.ANIMATION
        };
    }
}
