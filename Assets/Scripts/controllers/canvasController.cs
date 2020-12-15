using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvasController : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;
    Color healthBarColor;
    Color manaBarColor;
    public Image blackoutPanel;
    float barRestoreSpeed = 6f;

    private void Start()
    {
        //blackoutPanel.color = new Vector4(0, 0, 0, 1.0f);
        //doBlackout(false);
    }

    void Update()
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, resourceManager.Instance.getPlayerHealth(true), barRestoreSpeed * Time.deltaTime);
        manaSlider.value = Mathf.Lerp(manaSlider.value, resourceManager.Instance.getPlayerMana(true), barRestoreSpeed * Time.deltaTime);
        healthBarColor = healthSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color;
        manaBarColor = manaSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color;
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
}
