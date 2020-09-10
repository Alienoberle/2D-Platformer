using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private Transition transition;
    [SerializeField] 
    private List<Transition> transitionList;


    public void LoadLevel(string nameOfSceneToLoad, string transitionName)
    {
       foreach(Transition _transition in transitionList)
        {
            if(_transition.transitionName == transitionName)
            {
                transition.transitionAnimation = _transition.transitionAnimation;
                transition.transitionType = _transition.transitionType;
                transition.transitionTime = _transition.transitionTime;
            }
        }

        StartCoroutine(LevelTransition(nameOfSceneToLoad, transition.transitionAnimation, transition.transitionType, transition.transitionTime));
    }

    IEnumerator LevelTransition(string nameOfSceneToLoad, Animator transitionAnimation, string transitionType, float transitionTime)
    {
        transitionAnimation.SetTrigger(transitionType);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(nameOfSceneToLoad);
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
