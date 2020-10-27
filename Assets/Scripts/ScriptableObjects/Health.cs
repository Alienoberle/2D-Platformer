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

    public event Action<int, int> OnHealthChanged = delegate { };
    public event Action<ScriptableObject> OnHealthZero = delegate { };

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged(currentHealth, 0);
    }

    public void ModifyHealth(int healthChange)
    {
        currentHealth += healthChange;
        float currentHealthPercent = currentHealth / maxHealth;
        OnHealthChanged(currentHealth, healthChange);

        if (currentHealth < 1)
        {
            ScriptableObject scriptableObject = this;
            OnHealthZero(scriptableObject);
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
