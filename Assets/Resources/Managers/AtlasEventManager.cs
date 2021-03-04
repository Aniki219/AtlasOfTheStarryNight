using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/EventManager")]
public class AtlasEventManager : ScriptableObject
{
    private static AtlasEventManager instance;
    public static AtlasEventManager Instance { get { return instance; } }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<AtlasEventManager>("Managers")[0];
    }

    public event Action onBonkEvent;
    public void BonkEvent() { onBonkEvent?.Invoke(); }

    public event Action onBroomCancel;
    public void BroomCancelEvent() { onBroomCancel?.Invoke(); }
    
    public event Action onPlayerLand;
    public void PlayerLandEvent() { onPlayerLand?.Invoke(); }

    public event Action onFlagSet;
    public void FlagSetEvent() { onFlagSet?.Invoke(); }
}
