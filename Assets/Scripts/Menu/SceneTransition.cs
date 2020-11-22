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
    [Space(10)]
    [SerializeField] private List<Transition> transitionList;

    private SceneLoader sceneLoader;

    private void Awake()
    {
        sceneLoader = SceneLoader.instance;
    }
    private void EnableSceneTransitionCanvas()
    {
        canvas.enabled = true;
    }
    private void DisableSceneTransitionCanvas()
    {
        canvas.enabled = false;
    }
    public void LevelTransition(string transitionName, string transitionType)
    {
        EnableSceneTransitionCanvas();
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
        DisableSceneTransitionCanvas();
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
