using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;



public class MainMenu : MonoBehaviour
{

    public LevelLoader levelLoader;
    [SerializeField] UnityEvent OnPlayEvent;

    private void Awake()
    {
        if (levelLoader == null)
        {
            levelLoader = FindObjectOfType<LevelLoader>();
            Debug.Log("Levelloader was not assigned. Check MainMenu script.");
        }

    }

    public void Play()
    {
        levelLoader.LoadLevel("Testmap");
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
