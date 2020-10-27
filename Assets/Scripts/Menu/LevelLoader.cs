using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles the loading and transition from one scene to another
/// </summary>

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<LevelLoader>();
            if (_instance == null)
            {
                GameObject spawned = (GameObject)Instantiate(Resources.Load("LevelLoader"));
                //the spawned object's Awake() will run at this point, setting _instance to itself
            }
            return _instance;
        }
    }
    private static LevelLoader _instance;


    private Transition transition;
    [SerializeField]private List<Transition> transitionList;

    void Awake()
    {
        _instance = this;
    }

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
