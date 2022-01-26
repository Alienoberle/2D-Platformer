using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; }

    // Kick the game off with the first state
    void Start() => ChangeState(GameState.Starting);

    public void ChangeState(GameState newState)
    {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState)
        {
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.MainMenu:
                break;
            case GameState.PauseMenu:
                break;
            case GameState.ExitGame:
                HandleExitGame();
                break;
            case GameState.DebugMenu:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);

        Debug.Log($"New state: {newState}");
    }

    private void HandleStarting()
    {
        // Do some start setup, could be environment, cinematics etc
    }

    public void HandleExitGame()
    {
        Application.Quit();
        Debug.Log("Quite Game");
    }
}

[Serializable]
public enum GameState
{
    Starting = 0,
    MainMenu = 1,
    PauseMenu = 2,
    ExitGame = 3,
    DebugMenu = 4,
}