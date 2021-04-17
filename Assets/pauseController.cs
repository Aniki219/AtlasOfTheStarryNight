using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseController : MonoBehaviour
{
    public bool isPaused = false;
    public int priority = 0;

    public string scoutMessage = "";

    List<ComponentState> prevComponentStates;

    abstract class ComponentState
    {
        protected Component component;

        public abstract void disable();
        public abstract void revert();

        public Component getComponent()
        {
            return component;
        }
    }

    class ScriptState : ComponentState
    {
        bool wasEnabled;

        public ScriptState(MonoBehaviour script)
        {
            component = script;
        }

        public override void disable()
        {
            MonoBehaviour script = (MonoBehaviour)component;
            wasEnabled = script.enabled;
            script.enabled = false;
        }

        public override void revert()
        {
            if (!component) return;
            ((MonoBehaviour)component).enabled = wasEnabled;
        }
    }
    class AnimatorState : ComponentState
    {
        float prevSpeed;

        public AnimatorState(Animator anim)
        {
            component = anim;
        }

        override public void disable()
        {
            Animator anim = ((Animator)component);
            prevSpeed = anim.speed;
            anim.speed = 0;
        }

        override public void revert()
        {
            if (!component) return;
            Animator anim = ((Animator)component);
            anim.speed = prevSpeed;
        }
    }
    class ParticleSystemState : ComponentState
    {
        bool prevPlaying;

        public ParticleSystemState(ParticleSystem part)
        {
            component = part;
        }

        override public void disable()
        {
            ParticleSystem ps = ((ParticleSystem)component);
            prevPlaying = ps.isPlaying;
            if (prevPlaying) ps.Pause();
        }

        override public void revert()
        {
            if (!component) return;
            ParticleSystem ps = ((ParticleSystem)component);
            if (prevPlaying) ps.Play();
        }
    }
    class RigidbodyState : ComponentState
    {
        bool prevSimulated;

        public RigidbodyState(Rigidbody2D rb)
        {
            component = rb;
        }

        override public void disable()
        {
            Rigidbody2D rb = ((Rigidbody2D)component);
            prevSimulated = rb.simulated;
            rb.simulated = false;
        }

        override public void revert()
        {
            if (!component) return;
            ((Rigidbody2D)component).simulated = prevSimulated;
        }
    }

    private void Start()
    {
        prevComponentStates = new List<ComponentState>();
        gameManager.Instance.pause_manager.pauseEvent.AddListener(pause);
        gameManager.Instance.pause_manager.unpauseEvent.AddListener(unpause);
    }

    private void OnDestroy()
    {
        gameManager.Instance.pause_manager.pauseEvent.RemoveListener(pause);
        gameManager.Instance.pause_manager.unpauseEvent.RemoveListener(unpause);
    }

    public void pause(int pausePriority)
    {
        if (pausePriority < priority) return;
        if (transform.parent && transform.parent.GetComponentInParent<pauseController>()) return;

        //We iterate through every Component and manually place them into a list
        //Note that GetComponentsInChildren includes this object
        prevComponentStates = new List<ComponentState>();

        foreach (Component component in GetComponentsInChildren<Component>())
        {
            /*
             * For every component except the pauseController we create a new
             * ComponentState and add it to the list of prevComponentStates
             * addComponentState also disables each component. 
             * The tricky part is recording if the script was already disabled 
             * before this step. We need to then not enabled it when we unpause.
             * Also each script may need an OnDisabled to pause coroutines..TBD
             */
            switch (component) {
                case pauseController p:
                    continue;
                case MonoBehaviour mono:
                    addComponentState(new ScriptState(mono));
                    break;
                case Animator anim:
                    addComponentState(new AnimatorState(anim));
                    break;
                case Rigidbody2D rb:
                    addComponentState(new RigidbodyState(rb));
                    break;
                case ParticleSystem part:
                    addComponentState(new ParticleSystemState(part));
                    break;
            }  
        }
    }

    public void unpause(int pausePriority)
    {
        //If the current pause priority is of a same or higher value, don't unpause
        if (pausePriority >= priority) return;

        //Each ComponentState has a revert method that returns the component to its
        //previous state
        foreach (ComponentState componentState in prevComponentStates)
        {
            componentState.revert();
        }
    }

    //This helper method turns Components into ComponentStates
    //disabling the component is also what records previous state
    void addComponentState(ComponentState state)
    {
        if (state.getComponent() == null) return;
        state.disable();
        prevComponentStates.Add(state);
    }
}
