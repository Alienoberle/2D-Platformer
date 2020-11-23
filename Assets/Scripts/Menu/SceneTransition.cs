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

    public event Action OnTransitionFinish = delegate { };

    private void OnEnable()
    {
        sceneLoader.OnSceneLoadingStarted += TransitionOut;
        loadingscreen.OnLoadingscreenFinish += TransitionIn;
    }
    private void TransitionOut()
    {
        if (sceneLoader.showTransition)
        {
            canvas.enabled = true;
            LevelTransition(sceneLoader.transitionName, "TransitionOut");
        }
    }
    private void TransitionIn()
    {
        if (sceneLoader.showTransition)
        {
            canvas.enabled = true;
            LevelTransition(sceneLoader.transitionName, "TransitionIn");
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
        canvas.enabled = false;
        OnTransitionFinish();
    }
    private void OnDisable()
    {
        sceneLoader.OnSceneLoadingStarted -= TransitionOut;
        loadingscreen.OnLoadingscreenFinish -= TransitionIn;
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
