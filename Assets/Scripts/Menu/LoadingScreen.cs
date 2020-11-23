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
	
	[SerializeField] private SceneLoader sceneLoader;
	[SerializeField] private SceneTransition sceneTransition;

    private void OnEnable()
    {
		sceneLoader.OnSceneLoadingFinished += LoadingFinished;
    }
	public void EnableLoadingScreen() 
    {
        if (sceneLoader.showLoadingScreen)
        {
			canvas.enabled = true;
			StartCoroutine(UpdateLoadingProgressCoroutine());
			if (!minTimerRunning) StartCoroutine(MinLoadingTimeCoroutine());
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
			Debug.Log("Progress bar " + progressBar.fillAmount + "and value = " + totalProgress);

			yield return null;
		}
	}
	private void LoadingFinished()
    {
        if (!minTimerRunning) StartCoroutine(MinLoadingTimeCoroutine());
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
		StopCoroutine(UpdateLoadingProgressCoroutine());
		sceneTransition.TransitionIn(); 
		canvas.enabled = false;
	}
	private void OnDisable()
	{
		sceneLoader.OnSceneLoadingFinished -= DisableLoadingScreen;
	}
}
