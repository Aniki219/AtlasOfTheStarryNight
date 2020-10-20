using System;
using System.Collections.Generic;

public class StateMachine
{
    List<State> stateList;
    State currentState;

    public StateMachine(string entryState)
    {
        stateList = new List<State>();
        addStates(entryState);
        currentState = findState(entryState);
    }

    public void addStates(params string[] statenames)
    {
        for (int i = 0; i < statenames.Length; i++)
        {
            stateList.Add(new State(statenames[i]));
        }
    }

    //Get the name of the currentState as a string
    public string getState()
    {
        return currentState.getName();
    }

    //Search the StateMachine for a state with the given name
    //and assign that State as the currentState
    public void setState(string statename)
    {
        currentState = findState(statename);
    }

    //Searches the StateList for a State with a given name
    public State findState(string statename)
    {
        State rtnState = stateList.Find(s => s.getName() == statename);
        return rtnState;
    }

    public List<State> findStates(params string[] statenames)
    {
        List<State> rtnStates = new List<State>();
        for (int i = 0; i < statenames.Length; i++)
        {
            string statename = statenames[i];
            State foundState = stateList.Find(s => s.getName() == statename);
            if (foundState == null) throw new Exception("State: " + statename + " not found!");
            rtnStates.Add(foundState);
        }
        return rtnStates;
    }
}

public class State
{
    private string name;

    public State(string name)
    {
        this.name = name;
    }

    public string getName() { return name; }
}
