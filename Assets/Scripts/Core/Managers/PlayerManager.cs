using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : Singleton<PlayerManager>
{
    public List<GameObject> playerPrefabs;

    public static List<PlayerInput> playerList
    {
        get
        {
            return _playerList;
        }
    }
    private static List<PlayerInput> _playerList = new List<PlayerInput>();
    public void AddPlayer(PlayerInput player)=> _playerList.Add(player);
    public void RemovePlayer(PlayerInput player) => _playerList.Remove(player);
    public void SpawnPlayerAtPosition(int playerId, Vector2 spawnPosition)
    {
        Instantiate(playerPrefabs[playerId -1], spawnPosition, Quaternion.identity);
    }
}
