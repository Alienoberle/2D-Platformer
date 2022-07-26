using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    public UnityEvent PlayerEntersTrigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerEntersTrigger.Invoke();
            CheckpointTriggered();
        }
    }
    public void CheckpointTriggered()
    {
        foreach (var player in PlayerManager.playerList)
        {
            player.GetComponentInParent<Player>().lastCheckpoint = this.gameObject;
        }        
    }
}
