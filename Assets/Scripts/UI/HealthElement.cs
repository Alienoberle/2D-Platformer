using Ludiq;
using System;
using UnityEngine;
using UnityEngine.UI;


public class HealthElement : MonoBehaviour
{
    [SerializeField]
    private Health playerHealth;
    private int maxHealth;

    [SerializeField]
    private Color healthFull;
    [SerializeField]
    private Color healthEmpty;
    [SerializeField]
    private Image[] healthArray;


    private void OnEnable()
    {
        maxHealth = playerHealth.maxHealth;
        UpdateMaxHealth();
        UpdateHealthElement(99, 0);

        playerHealth.OnHealthChanged += UpdateHealthElement;
        playerHealth.OnHealthZero += OnHealthZero;
    }
    private void UpdateHealthElement(int currentHealth, int healthChange)
    {
        for (int i = 0; i < healthArray.Length; i++)
        {
            if (i < currentHealth)
            {
                healthArray[i].color = healthFull;
            }
            else
            {
                healthArray[i].color = healthEmpty;
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
    private void OnHealthZero(ScriptableObject obj)
    {
        // Play VFX and SFX on bar?
    }
    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthElement;
        playerHealth.OnHealthZero -= OnHealthZero;
    }
}
