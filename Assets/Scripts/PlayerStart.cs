using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStart : MonoBehaviour
{
    [SerializeField] private List<GameObject> PlayerSpawnpoints;

    private void Start()
    {
        // Spawn both players at their respective spawn location
        PlayerManager.Instance.SpawnPlayerAtPosition(1, PlayerSpawnpoints[0].transform.position);
        PlayerManager.Instance.SpawnPlayerAtPosition(2, PlayerSpawnpoints[1].transform.position);

        // Set the inital spawn positions as checkpoints
        PlayerManager.playerList[0].GetComponentInParent<Player>().lastCheckpoint = PlayerSpawnpoints[0].gameObject;
        PlayerManager.playerList[1].GetComponentInParent<Player>().lastCheckpoint = PlayerSpawnpoints[1].gameObject;
    }
}
