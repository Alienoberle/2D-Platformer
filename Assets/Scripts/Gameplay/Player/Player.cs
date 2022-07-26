using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerController playerController;
    private AudioSource audiosource;
    public Health playerHealth;
    public PlayerState playerState { get; private set; }
    [HideInInspector] public GameObject lastCheckpoint;
    private float invicibilityTimer = 3.0f;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioClip SfxTakeDamage;

    #region SetUp
    private void Awake()
    {
        playerManager = PlayerManager.Instance;
        playerController = GetComponentInParent<PlayerController>();
        audiosource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        playerHealth.OnHealthZero += HandleOnHealthZero;
    }
    private void OnDisable()
    {
        playerHealth.OnHealthZero -= HandleOnHealthZero;
    }
    #endregion
    #region PlayerHealth
    public void TakeDamage(int damage)
    {
        if (playerState == PlayerState.dead) return;
        if (playerState == PlayerState.invincible) return;
        playerHealth.ModifyHealth(damage);
        DamageFeedback();
    }
    private void DamageFeedback()
    {
        spriteRenderer.material.SetColor("_HitEffectColor", Color.white);
        spriteRenderer.material.SetFloat("_HitEffectGlow", 1.0f);
        spriteRenderer.material.DOFloat(1, "_HitEffectBlend", 0.2f).SetLoops(2, LoopType.Yoyo);
        audiosource.PlayOneShot(SfxTakeDamage);
    }
    private void HandleOnHealthZero(ScriptableObject obj)
    {
        print("dead");
        playerState = PlayerState.dead;
        playerController.enabled = false; 
        // disable physics
        // disable input
        // die animation 
        // die SFX
        
        Respawn();
    }
    #endregion
    #region Respawn
    private void Respawn()
    {
        playerHealth.SetHealth(1);
        playerController.enabled = true;
        gameObject.transform.position = lastCheckpoint.transform.position;
        StartCoroutine("InvicibilityCoroutine");
    }
    private IEnumerator InvicibilityCoroutine()
    {
        playerState = PlayerState.invincible;
        yield return new WaitForSeconds(invicibilityTimer);
        playerState = PlayerState.alive;
    }
    #endregion
    public enum PlayerState
    {
        alive,
        dead,
        invincible
    }
}
