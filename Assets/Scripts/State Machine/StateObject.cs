using System;
using System.Collections.Generic;
using UnityEngine;

public class StateObject : MonoBehaviour
{
    [SerializeReference, SubclassSelector]
    public List<IStateBehavior> stateBehaviors;
    public List<Allowances> allowances;
    public List<Resetters> resetters;

    public StateObject() {
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
