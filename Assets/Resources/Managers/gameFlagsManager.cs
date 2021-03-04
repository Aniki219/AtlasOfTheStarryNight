using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/GameFlags")]
public class gameFlagsManager : ScriptableObject
{
    private static gameFlagsManager instance;
    public static gameFlagsManager Instance { get { return instance; } }

    [System.Serializable]
    public class Flag
    {
        public string name;
        public bool set;

        public Flag(string name, bool set = false)
        {
            this.name = name;
            this.set = set;
        }
    }

    public Flag[] flags = new Flag[] {
        new Flag("GardenBombs"),
        new Flag("GardenBumps"),
        new Flag("GardenWoosh")
    };

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<gameFlagsManager>("Managers")[0];
    }

    public void setFlag(string name, bool set = true)
    {
        foreach (Flag f in flags)
        {
            if (f.name == name)
            {
                f.set = set;
                AtlasEventManager.Instance.FlagSetEvent();
                return;
            }
        }
        throw new System.Exception("No flag named " + name + " found!");
    }

    public bool checkFlag(string name)
    {
        foreach (Flag f in flags)
        {
            if (f.name == name)
            {
                return f.set;
            }
        }
        throw new System.Exception("No flag named " + name + " found!");
    }
}
