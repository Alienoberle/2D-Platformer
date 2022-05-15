using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class PickUp : MonoBehaviour
{

    private Animator _animator;
    private AudioSource _audiosource;
    public PickUpState state { get; private set; }

    [Header("Feedback")]
    [SerializeField] private AudioClip sFXPickUp;
    [SerializeField] private ParticleSystem vFXPickUp;

    [Header("Events")]
    [SerializeField] private UnityEvent OnPickUp;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audiosource = GetComponent<AudioSource>();
        Initialize();
    }
    private void Initialize()
    {
        _animator.Play("Idle");
        state = PickUpState.active;
    }
    private void Moving()
    {
        _animator.Play("MoveToPlayer");
        state = PickUpState.isMoving;
    }
    private void HandlePickUp()
    {
        _animator.Play("PickUp");
        GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * 0.9f;
        _audiosource.PlayOneShot(sFXPickUp);
        state = PickUpState.pickedUp;
    } 
    private void PickedUp() // Triggerd through Animation Event in "PickUp" animation
    {
        Instantiate(vFXPickUp, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == PickUpState.pickedUp) return;
        if (collision.CompareTag("Player"))
        {
            GetComponent<Collider2D>().enabled = false;
            HandlePickUp();
            OnPickUp.Invoke();
        } 
    }
    public enum PickUpState
    {
        active,
        isMoving,
        pickedUp
    }
}
