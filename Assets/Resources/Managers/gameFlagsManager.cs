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

    public Flag[] initflags = new Flag[] {
        new Flag("GardenBombs"),
        new Flag("GardenBumps"),
        new Flag("GardenWoosh")
    };

    private static Flag[] flags;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<gameFlagsManager>("Managers")[0];
        resetFlags();
    }

    public static void resetFlags()
    {
        flags = new Flag[instance.initflags.Length];
        for (int i = 0; i < instance.initflags.Length; i++)
        {
            flags[i] = new Flag(instance.initflags[i].name, instance.initflags[i].set);
        }
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
