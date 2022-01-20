using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the loading and transition from one scene to another
/// </summary>

public class SceneTransition : MonoBehaviour
{
    private Transition transition;
    [SerializeField] private GameObject sceneTransitionPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private SceneLoader sceneLoader;
    [Space(10)]
    [SerializeField] private List<Transition> transitionList;

    private bool isTransitionTypeOut;
    public event Action<bool> OnTransitionFinished = delegate { };

    public void TransitionOut()
    {
        canvas.enabled = true;
        LevelTransition(sceneLoader.transitionName, "TransitionOut");
        isTransitionTypeOut = true;
    }
    public void TransitionIn()
    {
        canvas.enabled = true;
        LevelTransition(sceneLoader.transitionName, "TransitionIn");
        isTransitionTypeOut = false;
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
        OnTransitionFinished(isTransitionTypeOut);
        canvas.enabled = false;   
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
