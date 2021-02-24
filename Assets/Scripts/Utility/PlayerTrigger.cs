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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            playerInTrigger = true;
            PlayerEntersTrigger.Invoke();
            OnTriggerEnter(this.transform.root.gameObject);
            Debug.Log("PlayerEnter");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            playerInTrigger = false;
            PlayerExitsTrigger.Invoke();
            OnTriggerExit(this.transform.root.gameObject);
            Debug.Log("PlayerExit");
        }
    }
}
