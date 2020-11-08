using Ludiq;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class provides funtionality for when the player reaches the levelgoal
/// </summary>
/// 
public class LevelGoal : MonoBehaviour
{
    private Player player;
    private GameManager GameManager;
    public event Action<GameObject> OnLevelGoalReached = delegate { };
    public UnityEvent OnTriggerEnter;

    private void Start()
    {
        player = Player.instance;
        GameManager = GameManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnTriggerEnter.Invoke();
            OnLevelGoalReached(this.transform.root.gameObject);
        }
    }
}
