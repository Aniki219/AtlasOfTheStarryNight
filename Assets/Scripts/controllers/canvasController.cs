using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvasController : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;

    float barRestoreSpeed = 6f; 

    void Update()
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, resourceManager.Instance.getPlayerHealth(true), barRestoreSpeed * Time.deltaTime);
        manaSlider.value = Mathf.Lerp(manaSlider.value, resourceManager.Instance.getPlayerMana(true), barRestoreSpeed * Time.deltaTime);
    }
}
