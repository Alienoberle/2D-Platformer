using UnityEngine;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GameObject levelLoaderPrefab;
    private GameObject levelLoaderInstance;

    //[SerializeField] private LevelLoader levelLoader;
    [SerializeField] UnityEvent OnPlayEvent;

    private void Awake()
    {
        // Make sure there is a level loader in the worst case spawn one
        if (levelLoader == null)
        { levelLoader = (LevelLoader)FindObjectOfType(typeof(LevelLoader)); }

        if (levelLoader == null)
        {
            levelLoaderInstance = Instantiate(levelLoaderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            levelLoader = levelLoaderInstance.GetComponent<LevelLoader>();
        }
    }

    public void Play()
    {
        levelLoader.LoadLevel("Testmap", "CrossfadeOut");
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
