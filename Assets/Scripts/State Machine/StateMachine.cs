using UnityEngine;
using System;
using System.Threading.Tasks;

public abstract class StateMachine : MonoBehaviour
{
    public State state { get; private set; } = new States.Init();
    public State startState;

    /*
        It seems I felt like this wasn't a good idea
        I think it is good to be able to inspect the
        state of any state machine simply by dragging
        a StateDisplay into the slot.
        
        Maybe I didn't like that nothing prevents us
        from having two StateMachines set to the same
        StateDisplay.
    */
    public StateDisplay stateDisplay;

    /*
        I think we can remove the concept of Phase
        or at least an explicit enum to track it
    */
    public Phase phase { get; protected set; } = Phase.Start;

    public enum Phase
    {
        Start,
        Update,
        Exit
    }

    /*
        Start on the StartState.
        Uses the State.Attach method to pass the StateMachine
        to the current state.
        Might want to remove this logic
    */
    public virtual void Start()
    {
        startState.Attach(this);
        ChangeState(startState);
    }

    /*
        Calls update unless game is paused and state is pausable
        Possibly unnecessary logic to prevent update being called
        on Start or Exit phase.
        We might need the concept of phase after all
    */
    public virtual void Update()
    {
        if (!phase.Equals(Phase.Update) && !state.alwaysUpdate) return;
        if (gameManager.isPaused) return;
        state.UpdateState();

        if (changeStateRequest.newState != null)
        {
            doChangeState(changeStateRequest.newState, changeStateRequest.skipWaitForExit);
            changeStateRequest = default;
        }
    }

    /*
        Seems to be used to request a state change and
        set some logic flags in the state
        Should be removed
    */
    struct ChangeStateRequest
    {
        public State newState;
        public bool skipWaitForExit;

        public ChangeStateRequest(State _newState, bool _skipWaitForExit)
        {
            newState = _newState;
            skipWaitForExit = _skipWaitForExit;
        }
    }

    /*
        Validates the StateChangeRequest to prevent restarting
        current active state.
        Weird logic to only create ChangeStateRequest on update
        Should remove logic and concept of ChangeStateRequest
    */
    ChangeStateRequest changeStateRequest;
    public void ChangeState(State newState, bool skipWaitForExit = false)
    {
        if (checkState(newState))
        {
            return;
        }
        if (phase.Equals(Phase.Update))
        {
            changeStateRequest = new ChangeStateRequest(newState, skipWaitForExit);
        }
        else
        {
            doChangeState(newState, skipWaitForExit);
        }
    }

    /*
        Interesting hack using Generics template to pass
        State Type when changing state.
        Seems fine as long as it isn't slow.
    */
    public void changeState<T>(bool skipWaitForExit = false) where T : State
    {
        ChangeState((T)Activator.CreateInstance(typeof(T)));
    }

    /* 
        Handles actually changing the state phase based on the
        ChangeStateRequest.
        There should be no need for this logic since states
        should no longer be async and therefore have no phase
        change logic.

        Previous note:
        Exit finishes and Start begins on the same Update tick
        right now I want to use this fact for linking together novadash behavior
    */
    private async void doChangeState(State newState, bool skipWaitForExit = false)
    {
        if (!state.skipExitState)
        {
            changePhase(Phase.Exit);
            if (skipWaitForExit)
            {
                _ = state.ExitState();
                await Task.Delay(16);
            }
            else
            {
                await state.ExitState();
            }
            state.PostExitState();
        }

        state = newState;
        newState.Attach(this);
        changePhase(Phase.Start);
        await state.StartState();
        changePhase(Phase.Update);
    }

    /*
        Changes phase from start -> update -> exit
        Should not be necessary when these are broken up into
        more states
    */
    protected virtual void changePhase(Phase to)
    {
        phase = to;
        if (stateDisplay)
        {
            stateDisplay.setPhase(to.ToString());
            stateDisplay.setStateInfo(state);
        }
    }


    /*
        Checks for equality between States
        Should be a comparator method on State
    */
    public bool checkState(State other)
    {
        return state.GetType().Equals(other.GetType());
    }
}