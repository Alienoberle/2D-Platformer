using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class handles the main menu functionality
/// </summary>

public class MainMenu : MonoBehaviour
{
    [SerializeField] private LoadEvent loadEvent;

    [Header("Play")]
    public GameScene[] scenesToLoad;

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
