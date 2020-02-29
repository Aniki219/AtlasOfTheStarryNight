using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Managers/ResourceManager")]
public class resourceManager : ScriptableObject
{
    private static resourceManager instance;
    public static resourceManager Instance { get { return instance; } }

    float playerHealth;
    float playerMana;

    [SerializeField] int playerMaxHealth = 5;
    [SerializeField] int playerMaxMana = 3;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<resourceManager>("Managers")[0];

        instance.playerMana = instance.playerMaxMana;
        instance.playerHealth = instance.playerMaxHealth;
    }

    public float getPlayerHealth(bool percentage = false)
    {
        return percentage ? (float)playerHealth / (float)playerMaxHealth : playerHealth;
    }

    public float getPlayerMana(bool percentage = false)
    {
        return percentage ? (float)playerMana / (float)playerMaxMana : playerMana;
    }

    public void restoreMana()
    {
        playerMana = playerMaxMana;
    }

    public void usePlayerMana(int amount)
    {
        playerMana -= amount;
        GameObject canv = GameObject.FindGameObjectWithTag("MainCanvas");
        if (canv) { 
            canv.SendMessage("FlashManaBar");
        }
    }

    public void takeDamage(int damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0) { playerHealth = 0; }
    }
}
