using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealthObject", menuName = "ScriptableObject/Health")]
public class Health : ScriptableObject, ISerializationCallbackReceiver
{
    public int maxHealth;
	[System.NonSerialized]
	public int currentHealth;

    public event Action<int> OnHealthChanged = delegate { };
    public event Action OnHealthZero = delegate { };

    private void Awake()
    {
        SetHealth(maxHealth);
    }
    public void ModifyHealth(int healthChange)
    {
        currentHealth += healthChange;
        float currentHealthPercent = currentHealth / maxHealth;
        CheckHealth(currentHealth);
    }
    public void SetHealth(int healthValue)
    {
        currentHealth = healthValue;
        float currentHealthPercent = currentHealth / maxHealth;
        CheckHealth(currentHealth);
    }
    private void CheckHealth(int currentHealth)
    {
        if (currentHealth > 0)
            OnHealthChanged(currentHealth);

        if (currentHealth <= 0)
        {
            OnHealthZero();
        }
    }
    public void OnAfterDeserialize()
	{
		currentHealth = maxHealth;
	}
	public void OnBeforeSerialize() 
    {
    
    }
}
