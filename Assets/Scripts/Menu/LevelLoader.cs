using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    public Animator defaultTransitionAnimation;

    [SerializeField]
    private string nameOfSceneToLoad;
   
    [SerializeField]
    private float transitionTime = 1.0f;


    public void LoadLevel(string nameOfSceneToLoad, Animator transitionAnimation)
    {
        StartCoroutine(LevelTransition(nameOfSceneToLoad, transitionAnimation));
    }

    public void LoadLevel(string nameOfSceneToLoad)
    {
        StartCoroutine(LevelTransition(nameOfSceneToLoad, defaultTransitionAnimation));
    }

    IEnumerator LevelTransition(string nameOfSceneToLoad, Animator transitionAnimation)
    {
        transitionAnimation.SetTrigger("StartTransitionIn");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(nameOfSceneToLoad);
    }

}
