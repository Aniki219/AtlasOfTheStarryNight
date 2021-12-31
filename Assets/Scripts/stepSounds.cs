using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stepSounds : MonoBehaviour
{
    public enum TerrainType
    {
        Smooth,
        Grass,
        Rock,
        Wood
    }

    public TerrainType terrainType = TerrainType.Smooth;
    public bool isSubmerged = false;

    public void playFootstep(int pitch = 0)
    {
        string path;
        if (isSubmerged)
        {
            path = "Footsteps/WaterStep";
        }
        else
        {
            path = "Footsteps/" + terrainType.ToString() + "Step";
        }
        SoundManager.Instance.playClip(path, pitch);
    }
}
