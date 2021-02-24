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
	
	[Header("Initialization Scene")]
	public GameScene initializationScene;
	[Header("Load on start")]
	public GameScene[] mainMenuScenes;

	[Header("Load Event")]
	[SerializeField] private LoadEvent _loadEvent = default; // The load event we are listening to
	private GameScene[] locationsToLoad;
	private List<AsyncOperation> _scenesToLoadAsyncOperations = new List<AsyncOperation>(); // List of the scenes to load and track progress
	private List<Scene> _ScenesToUnload = new List<Scene>();
	private GameScene _activeScene; // Keep track of the scene we want to set as active (for lighting/skybox)

	[SerializeField] private SceneTransition sceneTransition;
	[SerializeField] private LoadingScreen loadingScreen;

	public event Action OnSceneLoadingStarted = delegate { };
	public event Action OnSceneLoadingFinished = delegate { };
	public bool showLoadingScreen { get; private set; }
	public bool showTransition { get; private set; }
	public string transitionName { get; private set; }
	public float loadingProgress { get; private set; }

	private void Awake()
    {
		_instance = this;
		_loadEvent.loadEvent += TransitionAndLoadinscreenCheck;
	}
	private void Start()
	{
		if (SceneManager.GetActiveScene().name == initializationScene.sceneName)
		{
			LoadMainMenu();
		}
	}
	private void LoadMainMenu()
	{
		LoadScenes(mainMenuScenes);
	}
	private void TransitionAndLoadinscreenCheck(GameScene[] locationsToLoad)
	{
		showTransition = false;
		foreach (GameScene gameScene in locationsToLoad)
		{
			if (gameScene.showSceneTransition)
			{
				transitionName = gameScene.transitionName;
				showTransition = true;
				break;
			}
		}
		showLoadingScreen = false;
		foreach (GameScene gameScene in locationsToLoad)
		{
			if (gameScene.showLoadingScreen)
			{
				showLoadingScreen = true;
				break;
			}
		}
		this.locationsToLoad = locationsToLoad;
		StartTransitionOut();
	}

	private void StartTransitionOut()
    {
		if (showTransition)
        {
			if(showLoadingScreen)
            {
				sceneTransition.OnTransitionFinished += ShowLoadingScreen;
			}
			else
            {
				LoadScenes(locationsToLoad);
				OnSceneLoadingFinished += StartTransitionIn;
			}
			sceneTransition.TransitionOut();
		}
		else
        {
			ShowLoadingScreen(true);
		}
	}
	private void ShowLoadingScreen(bool isTransitionTypeOut)
	{
        if (isTransitionTypeOut)
        {
			LoadScenes(locationsToLoad);
			if (showLoadingScreen)
			{
				loadingScreen.OnLoadingScreenFinished += StartTransitionIn;
				loadingScreen.EnableLoadingScreen();
			}
            else
            {
				OnSceneLoadingFinished += StartTransitionIn;
			}
		}
		sceneTransition.OnTransitionFinished -= ShowLoadingScreen;
	}
	private void StartTransitionIn()
	{
		if (showTransition)
        {
			sceneTransition.TransitionIn();
		}
		loadingScreen.OnLoadingScreenFinished -= StartTransitionIn;
		OnSceneLoadingFinished -= StartTransitionIn;
	}
	///<summary> This function loads the scenes passed as array parameter </summary>
	public void LoadScenes(GameScene[] locationsToLoad)
    {
		AddScenesToUnload();
        _activeScene = locationsToLoad[0];

		for (int i = 0; i < locationsToLoad.Length; ++i)
        {
            String currentSceneName = locationsToLoad[i].sceneName;
            if (!CheckLoadState(currentSceneName))
            {
                //Add the scene to the list of scenes to load asynchronously in the background
                _scenesToLoadAsyncOperations.Add(SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive));
            }
        }
		
		OnSceneLoadingStarted();
		_scenesToLoadAsyncOperations[0].completed += SetActiveScene;
		_scenesToLoadAsyncOperations[locationsToLoad.Length - 1].completed += SceneLoadingCompleted;

		StartCoroutine(TrackLoadingProgress());
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
	IEnumerator TrackLoadingProgress()
	{
		loadingProgress = 0;
		//When the scene reaches 0.9f, it means that it is loaded
		//The remaining 0.1f are for the integration
		while (loadingProgress <= 0.9f)
		{

			//Reset the progress for the new values
			loadingProgress = 0;
			//Iterate through all the scenes to load
			for (int i = 0; i < _scenesToLoadAsyncOperations.Count; ++i)
			{
				//Debug.Log("Scene_" + i + ": Loaded = " + _scenesToLoadAsyncOperations[i].isDone + " Progress = " + _scenesToLoadAsyncOperations[i].progress);
				//Adding the scene progress to the total progress
				loadingProgress += _scenesToLoadAsyncOperations[i].progress;
			}
			yield return null;
		}
		//Clear the scenes to load
		_scenesToLoadAsyncOperations.Clear();
	}
	public void AddScenesToUnload()
	{
		for (int i = 0; i < SceneManager.sceneCount; ++i)
		{
			Scene scene = SceneManager.GetSceneAt(i);
			if (scene.name != initializationScene.sceneName)
			{
				//Debug.Log("Added scene to unload = " + scene.name);
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
    private void OnDestroy()
    {
		_loadEvent.loadEvent -= LoadScenes;
		loadingScreen.OnLoadingScreenFinished -= StartTransitionIn;
		sceneTransition.OnTransitionFinished -= ShowLoadingScreen;
	}
}