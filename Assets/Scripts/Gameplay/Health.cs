using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int maxHealth;
    public int currentHealth { get; private set; }

    public event Action<float, float> OnHealthChanged = delegate { };

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged(currentHealth, 0);
    }

    public void ModifyHealth(int healthChange)
    {
        currentHealth += healthChange;
        float currentHealthPercent = (float)currentHealth / (float)maxHealth;
        OnHealthChanged(currentHealth, healthChange);
    }
}
