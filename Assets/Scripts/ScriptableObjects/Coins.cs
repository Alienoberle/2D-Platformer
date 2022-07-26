using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Coins Object", menuName = "ScriptableObject/Coins")]
public class Coins : ScriptableObject, ISerializationCallbackReceiver
{
	[System.NonSerialized] public int coinCount = 0;
    public event Action OnCountChanged = delegate { };

    private void Awake()
    {

    }
    public void CoinCollected()
    {
        if (true)
        {
            UpdateCoinsCount();
        }
    }
    private void UpdateCoinsCount()
    {
        
        coinCount++;
        OnCountChanged();
    }
    public void OnAfterDeserialize()
    {
        //throw new NotImplementedException();
    }
    public void OnBeforeSerialize()
    {
        //throw new NotImplementedException();
    }
}
