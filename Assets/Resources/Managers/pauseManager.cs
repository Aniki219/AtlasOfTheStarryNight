using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class pauseManager : ScriptableObject
{
    //This is a list of pause reasons
    //the farther down the list the more "powerful" the pause. We say it has
    //higher "priority"
    public enum PauseType
    {
        Scouter,
        CutScene
    }

    //This is a Stack of alll of the current pause reasons
    //it will be ordered such that the highest priority is on top
    //An error is logged if a new pause type of equal or lower priority is added
    private Stack<PauseType> pauseStack;

    public UnityEvent<int> pauseEvent;
    public UnityEvent<int> unpauseEvent;

    public void Init()
    {
        pauseStack = new Stack<PauseType>();
        pauseEvent = new UnityEvent<int>();
        unpauseEvent = new UnityEvent<int>();
    }

    public void addPause(PauseType type)
    {
        int incomingPriority = (int)type;

        if (incomingPriority > getPriority())
        {
            pauseStack.Push(type);
            pauseEvent.Invoke(incomingPriority);
        } else
        {
            throw new System.Exception("Incomming pause type of priority: " + incomingPriority + " when current priority is: " + getPriority());
        }
    }

    public void unpause(PauseType type)
    {

        if (pauseStack.Peek() == type)
        {
            pauseStack.Pop();
            unpauseEvent.Invoke(getPriority());
        }
        else
        {
            throw new System.Exception("Received unpause type of priority: " + (int)type + " when current priority is: " + getPriority());
        }
    }

    private int getPriority()
    {
        return (pauseStack.Count > 0) ? (int)pauseStack.Peek() : -1;
    }
}
