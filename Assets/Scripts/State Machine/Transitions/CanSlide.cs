using UnityEngine;
using System;
using States;

namespace Transitions {
public class CanSlide : IStateTransition {
    public override void checkCondition() {
        if (AtlasInputManager.getAxisState("Dpad").y < 0 && AtlasInputManager.getKeyPressed("Jump"))
        {
            changeState(new Slide());
        } 
    }
}
}