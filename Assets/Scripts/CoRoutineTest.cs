using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CoRoutineTest : MonoBehaviour
{
    private IEnumerator update;

    [ContextMenu("StartIt")]
    private void StartIt()
    {
        if (update != null)
            StopCoroutine(update);

        update = UpdateCoroutine();
        StartCoroutine(update);
    }
    [ContextMenu("StopIt")]
    private void StopIt()
    {
        StopCoroutine(update);
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
