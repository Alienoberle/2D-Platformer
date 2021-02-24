using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CoRoutineTest : MonoBehaviour
{
    private IEnumerator countUpCoroutine;

    private void Start()
    {
        if (countUpCoroutine != null)
            StopCoroutine(countUpCoroutine);

        countUpCoroutine = UpdateCoroutine();
    }
    [ContextMenu("StartIt")]
    private void StartIt()
    {
        StartCoroutine(countUpCoroutine);
    }
    [ContextMenu("StopIt")]
    private void StopIt()
    {
        StopCoroutine(countUpCoroutine);
    }
    [ContextMenu("ClearIt")]
    private void ClearIt()
    {
        StopAllCoroutines();
    }

    private IEnumerator UpdateCoroutine()
    {
        int total = 0;
        while (true)
        {
            total++;
            Debug.Log(total);
            yield return new WaitForEndOfFrame();
        }
    }



}
