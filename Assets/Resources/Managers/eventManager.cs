using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/EventManager")]
public class eventManager : ScriptableObject
{
    private static eventManager instance;
    public static eventManager Instance { get { return instance; } }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<eventManager>("Managers")[0];
    }

    public event Action onBonkEvent;
    public void BonkEvent()
    {
        if (onBonkEvent != null)
        {
            onBonkEvent();
        }
    }

    public event Action onBroomCancel;
    public void BroomCancelEvent()
    {
        if (onBroomCancel != null)
        {
            onBroomCancel();
        }
    }
}
