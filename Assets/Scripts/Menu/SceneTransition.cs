using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class handles the loading and transition from one scene to another
/// </summary>

public class SceneTransition : MonoBehaviour
{
    private Transition transition;
    [SerializeField] private GameObject sceneTransitionPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private LoadingScreen loadingscreen;
    [Space(10)]
    [SerializeField] private List<Transition> transitionList;

    private bool lastTransitionTypeOut;

    private void OnEnable()
    {
        sceneLoader.OnSceneLoadingStarted += TransitionOut;
    }
    private void TransitionOut()
    {
        if (sceneLoader.showTransition)
        {
            canvas.enabled = true;
            LevelTransition(sceneLoader.transitionName, "TransitionOut");
            lastTransitionTypeOut = true;
        }
        else
        {
            loadingscreen.EnableLoadingScreen();
        }
    }
    public void TransitionIn()
    {
        if (sceneLoader.showTransition)
        {
            canvas.enabled = true;
            LevelTransition(sceneLoader.transitionName, "TransitionIn");
            lastTransitionTypeOut = false;
        }
    }
    public void LevelTransition(string transitionName, string transitionType)
    {
        foreach (Transition _transition in transitionList)
       {
            if(_transition.transitionName == transitionName && _transition.transitionType == transitionType)
            {
                transition.transitionType = _transition.transitionType;
                transition.transitionAnimation = _transition.transitionAnimation;
                transition.transitionTime = _transition.transitionTime;
            }
       }
        transition.transitionAnimation.SetTrigger(transition.transitionType);
        StartCoroutine(TransitionProgress(transition.transitionTime));
    }
    IEnumerator TransitionProgress(float transitionTime)
    {
        yield return new WaitForSeconds(transitionTime);
        TransitionFinished();
    }
    private void TransitionFinished()
    {
        canvas.enabled = false;
        if(lastTransitionTypeOut) loadingscreen.EnableLoadingScreen();
    }
    private void OnDisable()
    {
        sceneLoader.OnSceneLoadingStarted -= TransitionOut;
    }
    [Serializable] 
    public struct Transition
    {
        public string transitionName;
        public string transitionType;
        public Animator transitionAnimation;
        public float transitionTime;
    }
}
