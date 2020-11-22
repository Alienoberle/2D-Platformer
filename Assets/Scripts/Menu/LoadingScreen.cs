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

	private SceneLoader sceneLoader;
	//List of the scenes that are being loaded and progress tracked of
	private List<AsyncOperation> scenesToLoadAsyncOperations = new List<AsyncOperation>();

    private void Awake()
    {
		sceneLoader = SceneLoader.instance;
	}
	private void OnEnable()
	{
		sceneLoader.OnSceneLoadingFinished += DisableLoadingCanvas;
	}
	private void OnDisable()
	{
		sceneLoader.OnSceneLoadingFinished -= DisableLoadingCanvas;
	}
	public void ShowLoadingScreen(List<AsyncOperation> listOfScenesToLoadAsyncOperations)
    {
		EnableLoadingCanvas();
		scenesToLoadAsyncOperations = listOfScenesToLoadAsyncOperations;
		StartCoroutine(TrackLoadingProgressCoroutine());
	}
    private void EnableLoadingCanvas()
    {
		canvas.enabled = true;
    }
	private void DisableLoadingCanvas()
	{
		canvas.enabled = false;
	}
	IEnumerator TrackLoadingProgressCoroutine()
	{
		float totalProgress = 0;
		// When the scene reaches 0.9f, it means that it is loaded
		// The remaining 0.1f are for the integration
		while (totalProgress <= 0.9f)
		{

			// Reset the progress for the new values
			totalProgress = 0;
			// Iterate through all the scenes to load
			for (int i = 0; i < scenesToLoadAsyncOperations.Count; ++i)
			{
				Debug.Log("Scene" + i + " :" + scenesToLoadAsyncOperations[i].isDone + "progress = " + scenesToLoadAsyncOperations[i].progress);
				// Adding the scene progress to the total progress
				totalProgress += scenesToLoadAsyncOperations[i].progress;
			}
			// The fillAmount for all scenes, so we devide the progress by the number of scenes to load
			progressBar.fillAmount = totalProgress / scenesToLoadAsyncOperations.Count;
			Debug.Log("progress bar" + progressBar.fillAmount + "and value =" + totalProgress / scenesToLoadAsyncOperations.Count);

			yield return null;
		}

		scenesToLoadAsyncOperations.Clear();
		DisableLoadingCanvas();
	}
}
