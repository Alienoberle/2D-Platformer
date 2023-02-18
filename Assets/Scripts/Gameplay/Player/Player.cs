using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using System;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private AudioSource audiosource;
    public Health playerHealth;

    public PlayerState playerState { get; private set; }

    [HideInInspector] public GameObject lastCheckpoint;
    private float RespawnInvicibilityTime;


    #region SetUp
    private void Awake()
    {
        playerManager = PlayerManager.Instance;
        playerController = GetComponentInParent<PlayerController>();
        audiosource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChangePlayerState(PlayerState.alive);
    }
    private void OnEnable()
    {
        playerHealth.OnHealthChanged += TakeDamage;
        playerHealth.OnHealthZero += HandleOnHealthZero;
    }
    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= TakeDamage;
        playerHealth.OnHealthZero -= HandleOnHealthZero;
    }
    #endregion
    #region PlayerHealth
    public void TakeDamage(int damage)
    {
        if (playerState == PlayerState.dead) return;
        if (playerState == PlayerState.invincible) return;
        playerController.TakeDamage();
    }
    private void HandleOnHealthZero()
    {
        ChangePlayerState(PlayerState.dead);
    }
    #endregion
    #region Respawn
    private void Respawn() // Currently called from as event from within the "Hit" animation
    {
        gameObject.transform.position = lastCheckpoint.transform.position;
        ChangePlayerState(PlayerState.alive);
        StartCoroutine("InvicibilityCoroutine");
    }
    private IEnumerator InvicibilityCoroutine()
    {
        ChangePlayerState(PlayerState.invincible);
        yield return new WaitForSeconds(RespawnInvicibilityTime);
        ChangePlayerState(PlayerState.alive);
    }
    #endregion
    public void ChangePlayerState(PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.alive:
                playerState = PlayerState.alive;
                playerHealth.SetHealth(1);
                GetComponent<Rigidbody2D>().simulated = true;
                GetComponentInChildren<TrailRenderer>().enabled = true;
                //GetComponent<PlayerInputHandler>().enabled = true;
                break;
            case PlayerState.dead:
                playerState = PlayerState.dead;
                GetComponent<Rigidbody2D>().simulated = false;
                GetComponentInChildren<TrailRenderer>().enabled = false;
                playerController.Death();
               // GetComponent<PlayerInputHandler>().enabled = false;
                break;
            case PlayerState.invincible:
                playerState = PlayerState.invincible;
                spriteRenderer.enabled = true;
                spriteRenderer.material.SetColor("_HitEffectColor", Color.white);
                spriteRenderer.material.SetFloat("_HitEffectGlow", 1.0f);
                spriteRenderer.material.DOFloat(1, "_HitEffectBlend", 0.2f).SetLoops(18, LoopType.Yoyo);
                break;
            default:
                break;
        }
    }

    public enum PlayerState
    {
        alive,
        dead,
        invincible
    }
}
