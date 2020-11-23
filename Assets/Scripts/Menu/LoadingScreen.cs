using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	[Header("Loading Screen")]
	[SerializeField] private Canvas canvas;
	[SerializeField] private Image progressBar;
	[SerializeField] private float minLoadingScreenTime = 2.0f;
	private bool minTimerRunning = false;

	private SceneLoader sceneLoader;
	public event Action OnLoadingscreenFinish = delegate { };


	private void Awake()
    {
		sceneLoader = SceneLoader.instance;
	}
    private void OnEnable()
    {
		sceneLoader.OnSceneLoadingStarted += EnableLoadingScreen;
		sceneLoader.OnSceneLoadingFinished += DisableLoadingScreen;
    }
	public void EnableLoadingScreen() // REFACTOR SO IT WAITS UNTIL TRANSITION IS DONE
    {
        if (sceneLoader.showLoadingScreen)
        {
			canvas.enabled = true;
			StartCoroutine(TrackLoadingProgressCoroutine());
			StartCoroutine(MinLoadingScreenTimer());
		}
	}
	IEnumerator TrackLoadingProgressCoroutine()
	{
		float totalProgress = 0;
		
		// When the total progress reaches 0.9f, it means that it is loaded, the remaining 0.1f are for transition etc.
		while (totalProgress <= 0.9f)
		{
			totalProgress = sceneLoader.loadingProgress;

			// The fillAmount for all scenes, so we devide the progress by the number of scenes to load
			progressBar.fillAmount = totalProgress;
			Debug.Log("Progress bar " + progressBar.fillAmount + "and value = " + totalProgress);

			yield return null;
		}
	}
	IEnumerator MinLoadingScreenTimer()
    {
		minTimerRunning = true;
		yield return new WaitForSeconds(minLoadingScreenTime);
		minTimerRunning = false;
		DisableLoadingScreen();
	}
	private void DisableLoadingScreen()
	{
		if (!minTimerRunning)
        {
			OnLoadingscreenFinish();
			StopCoroutine(TrackLoadingProgressCoroutine());
			canvas.enabled = false;
		}
	}
	private void OnDisable()
	{
		sceneLoader.OnSceneLoadingStarted -= EnableLoadingScreen;
		sceneLoader.OnSceneLoadingFinished -= DisableLoadingScreen;
	}
}
