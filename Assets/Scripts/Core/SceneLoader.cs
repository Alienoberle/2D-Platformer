using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class manages the scenes loading and unloading
/// </summary>

public class SceneLoader : MonoBehaviour
{
	private static SceneLoader _instance;
	public static SceneLoader instance
	{
		get
		{
			if (_instance == null) _instance = FindObjectOfType<SceneLoader>();
			if (_instance == null)
			{
				GameObject spawned = (GameObject)Instantiate(Resources.Load("SceneLoader"));
			}
			return _instance;
		}
	}

	public event Action OnSceneLoadingStarted = delegate { };
	public event Action OnSceneLoadingFinished = delegate { };

	[Header("Initialization Scene")]
	public GameScene initializationScene;
	[Header("Load on start")]
	public GameScene[] mainMenuScenes;

	[Header("Load Event")]
	// The load event we are listening to
	[SerializeField] private LoadEvent _loadEvent = default;
	// List of the scenes to load and track progress
	private List<AsyncOperation> _scenesToLoadAsyncOperations = new List<AsyncOperation>();
	private List<Scene> _ScenesToUnload = new List<Scene>();
	// Keep track of the scene we want to set as active (for lighting/skybox)
	private GameScene _activeScene;

	[Header("Loading Screen")]
	[SerializeField] private LoadingScreen loadingScreen;
	private bool showLoadingScreen;

	[Header("Scene Transition")]
	[SerializeField] private SceneTransition sceneTransitionPrefab;
	private bool showTransition;
	private string transitionName;

	private void Awake()
    {
		_instance = this;
    }
	private void Start()
	{
		if (SceneManager.GetActiveScene().name == initializationScene.sceneName)
		{
			LoadMainMenu();
		}
	}
	private void OnEnable()
	{
		_loadEvent.loadEvent += LoadScenes;
	}
	private void OnDisable()
	{
		_loadEvent.loadEvent -= LoadScenes;
	}
	private void LoadMainMenu()
	{
		LoadScenes(mainMenuScenes);
	}
	///<summary> This function loads the scenes passed as array parameter </summary>
	public void LoadScenes(GameScene[] locationsToLoad)
	{
		//Add all current open scenes to unload list 
		AddScenesToUnload();

		_activeScene = locationsToLoad[0];
		OnSceneLoadingStarted();

		for (int i = 0; i < locationsToLoad.Length; ++i)
		{
			String currentSceneName = locationsToLoad[i].sceneName;
			if (!CheckLoadState(currentSceneName))
			{
				//Add the scene to the list of scenes to load asynchronously in the background
				_scenesToLoadAsyncOperations.Add(SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive));
			}

			if (locationsToLoad[i].showSceneTransition)
			{
				showTransition = true;
				transitionName = locationsToLoad[i].transitionName;
			}
			if (locationsToLoad[i].showLoadingScreen)
            {
				showLoadingScreen = true;
			}
		}

		//set the first scene in the list as active scene after it finished loading
		_scenesToLoadAsyncOperations[0].completed += SetActiveScene;

        _scenesToLoadAsyncOperations[locationsToLoad.Length - 1].completed += SceneLoadingCompleted;

        if (showTransition)
        {
            sceneTransitionPrefab.LevelTransition(transitionName, "TransitionIn");

        }
        if (showLoadingScreen)
        {
            loadingScreen.ShowLoadingScreen(_scenesToLoadAsyncOperations);
        }

        // clean up scenes to load list and unload no longer needed scenes
        _scenesToLoadAsyncOperations.Clear();
		UnloadScenes();
	}
    ///<summary> This function checks if a scene is already loaded </summary>
    public bool CheckLoadState(String sceneName)
	{
		for (int i = 0; i < SceneManager.sceneCount; ++i)
		{
			Scene scene = SceneManager.GetSceneAt(i);
			if (scene.name == sceneName)
			{
				return true;
			}
		}
		return false;
	}
	private void SetActiveScene(AsyncOperation asyncOp)
	{
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(_activeScene.sceneName));
	}
	private void SceneLoadingCompleted(AsyncOperation obj)
	{
		OnSceneLoadingFinished();
	}
	public void AddScenesToUnload()
	{
		for (int i = 0; i < SceneManager.sceneCount; ++i)
		{
			Scene scene = SceneManager.GetSceneAt(i);
			if (scene.name != initializationScene.sceneName)
			{
				Debug.Log("Added scene to unload = " + scene.name);
				//Add the scene to the list of the scenes to unload
				_ScenesToUnload.Add(scene);
			}
		}
	}
	public void UnloadScenes()
	{
		if (_ScenesToUnload != null)
		{
			for (int i = 0; i < _ScenesToUnload.Count; ++i)
			{
				//Unload the scene asynchronously in the background
				SceneManager.UnloadSceneAsync(_ScenesToUnload[i]);
			}
		}
		_ScenesToUnload.Clear();
	}
}