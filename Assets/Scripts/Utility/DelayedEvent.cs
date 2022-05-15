using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
    [SerializeField] private float delay;
    public UnityEvent delayedEvent;
    public void TriggerEvent()
    {
        StartCoroutine("Delay");
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        delayedEvent?.Invoke();
    }
}
