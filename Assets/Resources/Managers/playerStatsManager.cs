using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/StatsManager")]
public class playerStatsManager : ScriptableObject
{
    private static playerStatsManager instance;
    public static playerStatsManager Instance { get { return instance; } }

    public float gravity;
}
