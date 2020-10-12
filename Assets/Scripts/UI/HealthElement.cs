using Ludiq;
using System;
using UnityEngine;
using UnityEngine.UI;


public class HealthElement : MonoBehaviour
{
    [SerializeField]
    private Health playerHealth;
    private int maxHealth;

    //[SerializeField]
    //private Sprite healthEmpty;
    //[SerializeField]
    //private Sprite healthFull;
    [SerializeField]
    private Color healthFull;
    [SerializeField]
    private Color healthEmpty;
    [SerializeField]
    private Image[] healthArray;




    private void Awake()
    {
        maxHealth = playerHealth.maxHealth;
        UpdateMaxHealth();
        UpdateHealthElement(99, 0);

        playerHealth.OnHealthChanged += UpdateHealthElement;
        playerHealth.OnHealthZero += OnHealthZero;
    }

    private void UpdateHealthElement(float currentHealth, float healthChange)
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

    private void OnHealthZero(GameObject obj)
    {
        Debug.Log("DEAD!");
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthElement;
        playerHealth.OnHealthZero -= OnHealthZero;
    }
}
