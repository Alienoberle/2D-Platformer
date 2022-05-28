using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class provides funtionality for when the player enters or leaves the 2D Collider
/// </summary>
/// 
public class MagnetTrigger : MonoBehaviour
{
    public bool magnetInTrigger { get; private set; } = false;

    [Header("Events")]
    public UnityEvent PlayerEntersTrigger;
    public UnityEvent PlayerExitsTrigger;
    public event Action<GameObject> OnTriggerEnter = delegate { };
    public event Action<GameObject> OnTriggerExit = delegate { };

    public bool isMagnetInTrigger => magnetInTrigger == true;

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
        if (collision.CompareTag("Magnet"))
        {
            magnetInTrigger = true;
            Enter();
            OnTriggerEnter(transform.root.gameObject);
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnet"))
        {
            magnetInTrigger = true;
            Enter();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnet"))
        {
            Exit();
            OnTriggerExit(transform.root.gameObject);
        }
    }
}
