using UnityEngine;
using System;
using States;

namespace Transitions {
public class CanSlide : IStateTransition {
    public override void checkCondition() {
        if (AtlasInputManager.getKeyPressed("Jump"))
        {
            changeState(new Slide());
        } 
    }
}
}