using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int maxHealth;
    public int currentHealth { get; private set; }

    public event Action<float, float> OnHealthChanged = delegate { };
    public event Action<GameObject> OnHealthZero = delegate { };

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

        if(currentHealth < 1)
        {
            GameObject parent = this.transform.root.gameObject;
            OnHealthZero(parent);
        }
    }
}
