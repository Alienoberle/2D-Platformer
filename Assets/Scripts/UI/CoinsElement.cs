using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinsElement : MonoBehaviour
{
    [SerializeField] private Coins coins;
    private TextMeshProUGUI coinsAmount;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        coinsAmount = GetComponentInChildren<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        UpdateCoinElement();
        coins.OnCountChanged += UpdateCoinElement;
    }
    private void UpdateCoinElement()
    {
        coinsAmount.text = coins.coinCount.ToString();
        animator.Play("UpdateCoin");
    }
    private void OnDisable()
    {
        coins.OnCountChanged -= UpdateCoinElement;
    }
}
