using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class clothesIconController : MonoBehaviour
{
    public void setPlayerMaterial()
    {
        Material mat = GetComponent<Image>().material;
        playerStatsManager.Instance.currentSkin = mat;
    }
}
