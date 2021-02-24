using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	[Header("Loading Screen")]
	[SerializeField] private Canvas canvas;
	[SerializeField] private Image progressBar;
	[SerializeField] private float minLoadingScreenTime = 2.0f;
	[SerializeField] private SceneLoader sceneLoader;

	private bool minTimerRunning;
	private bool loadingFinished;

	public event Action OnLoadingScreenFinished = delegate { };

	private void OnEnable()
    {
		//sceneLoader.OnSceneLoadingFinished += LoadingFinished;
    }
	public void EnableLoadingScreen() 
    {
		sceneLoader.OnSceneLoadingFinished += LoadingFinished;
		minTimerRunning = false;
		loadingFinished = false;
		canvas.enabled = true;
		StartCoroutine(UpdateLoadingProgressCoroutine());
	
		if (!minTimerRunning)
			StartCoroutine(MinLoadingTimeCoroutine());
	}
	private void LoadingFinished()
    {
		loadingFinished = true;
		DisableLoadingScreen();
	}
	IEnumerator MinLoadingTimeCoroutine()
    {
		minTimerRunning = true;
		yield return new WaitForSeconds(minLoadingScreenTime);
		minTimerRunning = false;
		DisableLoadingScreen();
	}
	public void DisableLoadingScreen()
	{
		if (loadingFinished && !minTimerRunning)
        {
			OnLoadingScreenFinished();
			StopCoroutine(UpdateLoadingProgressCoroutine());
			canvas.enabled = false;
			sceneLoader.OnSceneLoadingFinished -= LoadingFinished;
		}
	}
	IEnumerator UpdateLoadingProgressCoroutine()
	{
		float totalProgress = 0;

		// When the total progress reaches 0.9f, it means that it is loaded, the remaining 0.1f are for transition etc.
		while (totalProgress <= 0.9f)
		{
			totalProgress = sceneLoader.loadingProgress;

			// The fillAmount for all scenes, so we devide the progress by the number of scenes to load
			progressBar.fillAmount = totalProgress;
			//Debug.Log("Progress bar " + progressBar.fillAmount + "and value = " + totalProgress);

			yield return null;
		}
	}
}
