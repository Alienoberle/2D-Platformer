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

    [Header("Events")]
    public UnityEvent PlayerEntersTrigger;
    public UnityEvent PlayerExitsTrigger;
    public UnityEvent BothPlayerEnteredTrigger;
    public UnityEvent BothPlayersExitTrigger;
    public event Action<GameObject> OnTriggerEnter = delegate { };
    public event Action<GameObject> OnTriggerExit = delegate { };

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
    private void Update()
    {
        playerInTrigger = (numPlayersInTrigger > 0) ? true : false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Mathf.Clamp(numPlayersInTrigger++, 0, 2);
            Enter();
            OnTriggerEnter(transform.root.gameObject);
        }
        if(numPlayersInTrigger == 2)
        {
            BothEntered();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = true;
            Enter();
            OnTriggerEnter(transform.root.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Mathf.Clamp(numPlayersInTrigger--,0,2);
            Exit();
            OnTriggerExit(transform.root.gameObject);
        }
        if (numPlayersInTrigger == 0)
        {
            BothExited();
        }
    }
}
