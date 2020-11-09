using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/InputManager")]
public class inputManager : ScriptableObject
{
    private static inputManager instance;
    public static inputManager Instance { get { return instance; } }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<inputManager>("Managers")[0];
    }
}
