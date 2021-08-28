using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenTest : MonoBehaviour
{

    public float weight;
    // Start is called before the first frame update
    void Start()
    {
        DOTween.To(() => weight, x => weight = x, 60, 60);
    }

}
