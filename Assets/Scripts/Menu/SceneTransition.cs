using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles the loading and transition from one scene to another
/// </summary>

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private GameObject sceneTransitionPrefab;
    [SerializeField] private Transition transition;
    [SerializeField] private List<Transition> transitionList;
    [SerializeField] private UnityEvent onTransitionFinished;

    public void LevelTransition(string transitionName)
    {
        sceneTransitionPrefab.SetActive(true);
        foreach (Transition _transition in transitionList)
       {
            if(_transition.transitionName == transitionName)
            {
                transition.transitionAnimation = _transition.transitionAnimation;
                transition.transitionType = _transition.transitionType;
                transition.transitionTime = _transition.transitionTime;
            }
       }
        transition.transitionAnimation.SetTrigger(transition.transitionType);
        StartCoroutine(TransitionProgress(transition.transitionTime));
    }

    IEnumerator TransitionProgress(float transitionTime)
    {
        yield return new WaitForSeconds(transitionTime);
        sceneTransitionPrefab.SetActive(false);
        onTransitionFinished.Invoke();
        Debug.Log("Transition Finished");
    }

    [Serializable]
    public struct Transition
    {
        public string transitionName;
        public Animator transitionAnimation;
        public string transitionType;
        public float transitionTime;
    }
}
