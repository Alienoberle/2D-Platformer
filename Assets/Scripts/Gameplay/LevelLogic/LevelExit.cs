using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private LoadEvent loadEvent;
    [SerializeField] public GameScene[] nextScenes;

    
    public void Exit()
    {
        Load();
        var players = FindObjectsOfType<Player>();
        foreach(Player player in players)
        {
            Destroy(player);
        }
    }
    
    private void Load()
    {
        loadEvent.Raise(nextScenes);
    }

}
