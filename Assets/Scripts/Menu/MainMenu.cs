using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// This class handles the main menu functionality
/// </summary>

public class MainMenu : MonoBehaviour
{
    [SerializeField] private LoadEvent loadEvent;
    [SerializeField] private GameObject firstSelected;

    [Header("Play")]
    public GameScene[] scenesToLoad;

    public void OpenMainMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void Play()
    {
        loadEvent.Raise(scenesToLoad);
        Debug.Log("Play");
    }

    public void Options()
    {
        Debug.Log("Options Menu");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }


}
