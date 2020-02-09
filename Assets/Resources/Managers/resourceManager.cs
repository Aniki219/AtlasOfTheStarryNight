using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Managers/ResourceManager")]
public class resourceManager : ScriptableObject
{
    private static resourceManager instance;
    public static resourceManager Instance { get { return instance; } }

    [HideInInspector] public float playerHealth;
    [HideInInspector] public float playerMana;

    public int playerMaxHealth = 5;
    public int playerMaxMana = 3;

    public int barRestoreSpeed = 6;

    static GameObject canvas;
    static Slider healthBar;
    static Slider manaBar;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<resourceManager>("Managers")[0];
        canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        manaBar = canvas.transform.Find("ManaBar").GetComponent<Slider>();
        healthBar = canvas.transform.Find("HealthBar").GetComponent<Slider>();

        instance.playerMana = instance.playerMaxMana;
        instance.playerHealth = instance.playerMaxHealth;
    }

    public void setResourceBars()
    {
        float manaPercent = (float)playerMana / (float)playerMaxMana;
        float healthPercent = (float)playerHealth / (float)playerMaxHealth;
        manaBar.value = Mathf.Lerp(manaBar.value, manaPercent, barRestoreSpeed * Time.deltaTime);
        healthBar.value = Mathf.Lerp(healthBar.value, healthPercent, barRestoreSpeed * Time.deltaTime);
    }

    public void restoreMana()
    {
        playerMana = playerMaxMana;
    }

    public void takeDamage(int damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0) { playerHealth = 0; }
    }
}
