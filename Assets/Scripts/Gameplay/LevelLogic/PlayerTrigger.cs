using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class provides funtionality for when the player enters or leaves the 2D Collider
/// </summary>
/// 
public class PlayerTrigger : MonoBehaviour
{
    public int numPlayersInTrigger { get; private set; } = 0;
    public bool playerInTrigger { get; private set; } = false;
    
    [System.Serializable]
    public class TriggerEvent : UnityEvent<GameObject>
    {
    }

    [Header("Events")]
    public UnityEvent PlayerEntersTrigger;
    public UnityEvent PlayerExitsTrigger;
    public UnityEvent BothPlayerEnteredTrigger;
    public UnityEvent BothPlayersExitTrigger;
    public event Action<GameObject> OnTriggerEnter = delegate { };
    public event Action<GameObject> OnTriggerExit = delegate { };

    public bool isPlayerInTrigger => (numPlayersInTrigger > 0) ? true : false;

    [ContextMenu("Enter")]
    private void Enter()
    {
        PlayerEntersTrigger.Invoke();
    }
    [ContextMenu("Exit")]
    private void Exit()
    {
        PlayerExitsTrigger.Invoke();
    }
    [ContextMenu("BothEntered")]
    private void BothEntered()
    {
        BothPlayerEnteredTrigger.Invoke();
    }
    [ContextMenu("BothExited")]
    private void BothExited()
    {
        BothPlayersExitTrigger.Invoke();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Mathf.Clamp(numPlayersInTrigger++, 0, 2);
            Enter();
            OnTriggerEnter(gameObject);
        }
        if(numPlayersInTrigger == 2)
        {
            BothEntered();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Mathf.Clamp(numPlayersInTrigger--,0,2);
            Exit();
            OnTriggerExit(gameObject);
        }
        if (numPlayersInTrigger == 0)
        {
            BothExited();
        }
    }
}
