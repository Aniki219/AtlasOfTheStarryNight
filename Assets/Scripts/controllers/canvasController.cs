using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider novaSlider;
    private List<ResourceBar> resourceBars;
    Color healthBarColor;
    Color manaBarColor;
    public Image blackoutPanel;
    float barRestoreSpeed = 6f;

    private void Start()
    {
        //blackoutPanel.color = new Vector4(0, 0, 0, 1.0f);
        //doBlackout(false);
        resourceBars = new List<ResourceBar>() {
            new ResourceBar(novaSlider, ResourceManager.Instance.nova),
        };
    }

    void Update()
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, ResourceManager.Instance.getPlayerHealth(true), barRestoreSpeed * Time.deltaTime);
        manaSlider.value = Mathf.Lerp(manaSlider.value, ResourceManager.Instance.getPlayerMana(true), barRestoreSpeed * Time.deltaTime);
        healthBarColor = healthSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color;
        manaBarColor = manaSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color;

        resourceBars.ForEach(rb => rb.UpdateSliderValue());
    }

    public void FlashManaBar()
    {
        StartCoroutine(flashBar("mana"));
    }

    IEnumerator flashBar(string bar = "mana")
    {
        Color startColor = manaBarColor;

        float startTime = Time.time;
        Image img = manaSlider.transform.Find("Fill Area/Fill").GetComponent<Image>();

        while (Time.time - startTime < 0.4f)
        {
            img.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            img.color = new Color(startColor.r, startColor.g, startColor.b, startColor.a);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void doBlackout(bool fadeout = true)
    {
        StartCoroutine(blackout(fadeout));
    }

    IEnumerator blackout(bool fadeout)
    {
        blackoutPanel.color = new Vector4(0, 0, 0, fadeout ? 0 : 1.0f);
        Vector4 targetColor = new Vector4(0, 0, 0, fadeout ? 1.0f : 0);

        float duration = 1.0f;
        float elapsed = 0;
        while ((elapsed += Time.deltaTime) < duration)
        {
            blackoutPanel.color = Vector4.Lerp(blackoutPanel.color, targetColor, elapsed / duration);
            yield return new WaitForEndOfFrame();
        }
        blackoutPanel.color = targetColor;
        yield return 0;
    }

    public class ResourceBar {
        Slider slider;
        ResourceManager.Resource resource;
        float targetValue;
        float startTime;
        float smoothingTime = 0.25f;

        public ResourceBar(Slider _slider, ResourceManager.Resource _resourceReference) {
            slider = _slider;
            resource = _resourceReference;
            resource.OnChange.AddListener(SetTargetValue);
            startTime = Time.time;
        }

        private void SetTargetValue(float delta) {
            if (targetValue == slider.value) startTime = Time.time;
            targetValue = resource.getPercentage();
        }

        public void UpdateSliderValue() {
            float currentTime = Time.time - startTime;
            slider.value = EasingFunctions.EaseOutQuart(slider.value, targetValue, Mathf.Clamp01(currentTime/smoothingTime));
            //Debug.Log("Update Slider Value to: " + targetValue + " currently: " + slider.value + " | t: " + currentTime);
        }
    }
}
