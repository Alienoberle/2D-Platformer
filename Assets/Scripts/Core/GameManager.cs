using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton set up
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>();
            if (_instance == null)
            {
                GameObject spawned = (GameObject)Instantiate(Resources.Load("GameManager"));
                //the spawned object's Awake() will run at this point, setting _instance to itself
            }
            return _instance;
        }
    }

    public bool gameIsRunning { get; set; }
    public bool gameIsPaused { get; set; }

    private void Awake()
    {
        _instance = this;
    }



    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quite Game");
    }
}
