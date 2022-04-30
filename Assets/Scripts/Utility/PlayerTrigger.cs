using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class provides funtionality for when the player enters or leaves the 2D Collider
/// </summary>
/// 
public class PlayerTrigger : MonoBehaviour
{
    public bool playerInTrigger { get; private set; } = false;
    public event Action<GameObject> OnTriggerEnter = delegate { };
    public event Action<GameObject> OnTriggerExit = delegate { };
    public UnityEvent PlayerEntersTrigger;
    public UnityEvent PlayerExitsTrigger;

    public bool isPlayerInTrigger => playerInTrigger == true;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = true;
            Enter();
            OnTriggerEnter(this.transform.root.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
            Exit();
            OnTriggerExit(this.transform.root.gameObject);
        }
    }
}
