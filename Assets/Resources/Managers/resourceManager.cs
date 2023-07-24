using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Managers/ResourceManager")]
public class ResourceManager : ScriptableObject
{
    private static ResourceManager instance;
    public static ResourceManager Instance { get { return instance; } }

    float playerHealth;
    float playerMana;

    public Resource nova {get; private set;}

    [SerializeField] int playerMaxHealth = 5;
    [SerializeField] int playerMaxMana = 3;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<ResourceManager>("Managers")[0];

        instance.playerMana = instance.playerMaxMana;
        instance.playerHealth = instance.playerMaxHealth;

        instance.nova = new Resource(100, 0);
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

    public class Resource {
        public float value {get; private set;}
        public float max {get; private set;}

        public UnityEvent<float> OnChange = new UnityEvent<float>();
        public UnityEvent OnNotEnoughResource = new UnityEvent();

        public Resource(float _max) {
            max = _max;
            value = max;
        }

        public Resource(float _max, float initialValue) {
            max = _max;
            value = initialValue;
        }

        public void plus(float delta) {
            float startingAmount = value;
            value = Mathf.Clamp(value + delta, 0, max);

            if (value - startingAmount == 0) return;
            OnChange.Invoke(value - startingAmount);
        }

        public void minus(float delta) {
            plus(-delta);
        }

        public bool TryMinus(float delta) {
            if (value > delta) {
                plus(-delta);
                return true;
            }
            OnNotEnoughResource.Invoke();
            return false;
        }

        public bool HasAtleast(float amount) {
            return value >= amount;
        }

        public float getPercentage() {
            return Mathf.Clamp01(value/max);
        }

        public void setValue(float to) {
            plus(to - value);
        }
    }
}
