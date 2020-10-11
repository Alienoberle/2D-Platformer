using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;
using System;

public class HealthElement : MonoBehaviour
{
    [SerializeField]
    private Health playerHealth;
    [SerializeField]
    private int maxHealth;

    [SerializeField]
    private Sprite healthEmpty;
    [SerializeField]
    private Sprite healthFull;
    [SerializeField]
    private Image[] healthArray;




    private void Awake()
    {
        maxHealth = playerHealth.maxHealth;
        UpdateMaxHealth();
        UpdateHealthElement(99, 0);

        playerHealth.OnHealthChanged += UpdateHealthElement;
    }

    private void UpdateHealthElement(float currentHealth, float healthChange)
    {
        for (int i = 0; i < healthArray.Length; i++)
        {
            if (i < currentHealth)
            {
                healthArray[i].sprite = healthFull;
            }
            else
            {
                healthArray[i].sprite = healthEmpty;
            }
        }
    }

    private void UpdateMaxHealth()
    {
        for (int i = 0; i < healthArray.Length; i++)
        {
            if (i < maxHealth)
            {
                healthArray[i].enabled = true;
            }
            else
            {
                healthArray[i].enabled = false;
            }
        }
    }
}
