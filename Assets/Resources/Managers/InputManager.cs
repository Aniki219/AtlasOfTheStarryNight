using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/InputManager")]
public class InputManager : ScriptableObject
{
    private static InputManager instance;
    public static InputManager Instance { get { return instance; } }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<InputManager>("Managers")[0];
    }
}
