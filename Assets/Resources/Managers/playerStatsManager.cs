using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/StatsManager")]
public class playerStatsManager : ScriptableObject
{
    private static playerStatsManager instance;
    public static playerStatsManager Instance { get { return instance; } }

    public Material[] playerSkins;
    public Material currentSkin;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<playerStatsManager>("Managers")[0];
    }
}
