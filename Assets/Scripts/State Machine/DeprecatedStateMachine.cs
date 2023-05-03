using System;
using System.Collections.Generic;

public class DeprecatedStateMachine
{
    List<DeprecatedState> stateList;
    DeprecatedState currentState;

    public DeprecatedStateMachine(string entryState)
    {
        stateList = new List<DeprecatedState>();
        addStates(entryState);
        currentState = findState(entryState);
    }

    public void addStates(params string[] statenames)
    {
        for (int i = 0; i < statenames.Length; i++)
        {
            stateList.Add(new DeprecatedState(statenames[i]));
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
    public DeprecatedState findState(string statename)
    {
        DeprecatedState rtnState = stateList.Find(s => s.getName() == statename);
        if (rtnState == null) throw new Exception("State: " + statename + " not found!");
        return rtnState;
    }

    public List<DeprecatedState> findStates(params string[] statenames)
    {
        List<DeprecatedState> rtnStates = new List<DeprecatedState>();
        for (int i = 0; i < statenames.Length; i++)
        {
            string statename = statenames[i];
            DeprecatedState foundState = stateList.Find(s => s.getName() == statename);
            if (foundState == null) throw new Exception("State: " + statename + " not found!");
            rtnStates.Add(foundState);
        }
        return rtnStates;
    }
}

public class DeprecatedState
{
    private string name;

    public DeprecatedState(string name)
    {
        this.name = name;
    }

    public string getName() { return name; }
}
