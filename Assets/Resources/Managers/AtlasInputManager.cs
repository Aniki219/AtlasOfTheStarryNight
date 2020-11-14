using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Managers/AtlasInputManager")]
public class AtlasInputManager : ScriptableObject
{
    private static AtlasInputManager instance;
    public static AtlasInputManager Instance { get { return instance; } }

    public static List<KeyState> keyStates;
    public static List<AxisState> axisStates;

    public InputMaster controls;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<AtlasInputManager>("Managers")[0];
        instance.controls = new InputMaster();
        instance.controls.Enable();
        InputMaster.PlayerActions input = instance.controls.Player;

        axisStates = new List<AxisState>();
        axisStates.Add(new AxisState("Dpad"));

        InputActionMap actionMap = instance.controls.Player;
        keyStates = new List<KeyState>();
        foreach (InputAction action in actionMap.actions) {
            string key = action.name;
            keyStates.Add(new KeyState(key));
            action.started += ctx => { setKeyState(key, true); };
            action.canceled += ctx => { setKeyState(key, false); };
        }
        input.Movement.started += ctx => { setAxisState("Dpad", ctx.ReadValue<Vector2>()); };
        input.Movement.performed += ctx => { setAxisState("Dpad", ctx.ReadValue<Vector2>()); };
        input.Movement.canceled += ctx => { setAxisState("Dpad", ctx.ReadValue<Vector2>()); };
    }

    static void setKeyState(string keyName, bool state)
    {
        KeyState keyState = keyStates.Find(ks => ks.keyName == keyName);
        if (keyState == null) throw new Exception("No keyState " + keyName + " Found!");
        keyState.state = state;
        keyState.startTime = Time.time;
    }

    static void setAxisState(string axisName, Vector2 state)
    {
        AxisState axisState = axisStates.Find(axis => axis.axisName == axisName);
        if (axisState == null) throw new Exception("No axisState " + axisName + " Found!");
        axisState.state = state;
        axisState.startTime = Time.time;
    }

    public static KeyState getKeyState(string keyName)
    {
        KeyState keyState = keyStates.Find(ks => ks.keyName == keyName);
        if (keyState == null) throw new Exception("No keyState " + keyName + " Found!");
        return keyState;
    }

    public static Vector2 getAxisState(string axisName)
    {
        AxisState axisState = axisStates.Find(axis => axis.axisName == axisName);
        if (axisState == null) throw new Exception("No axisState " + axisName + " Found!");
        return axisState.state;
    }

    public static bool getKeyPressed(string keyName)
    {
        KeyState keyState = getKeyState(keyName);
        return keyState.state && keyState.justPressed();
    }

    public static bool getKey(string keyName)
    {
        return getKeyState(keyName).state;
    }
}

public class KeyState
{
    public string keyName;
    public bool state;
    public double startTime;

    public KeyState(string keyName)
    {
        this.keyName = keyName;
        state = false;
    }

    public bool justPressed()
    {
        return (Time.time - startTime <= 0);
    }
}

public class AxisState
{
    public string axisName;
    public Vector2 state;
    public double startTime;

    public AxisState(string axisName)
    {
        this.axisName = axisName;
        state = Vector2.zero;
    }

    public bool justPressed()
    {
        return (Time.time - startTime <= 0);
    }
}
