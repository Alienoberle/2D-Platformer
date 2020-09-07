using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public LevelLoader levelLoader;

    private void Awake()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
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
