using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Feedbacks;

public class PickUp : MonoBehaviour
{
    private Animator _animator;
    
    public PickUpState state { get; private set; }

    [Header("Feedback")]
    [SerializeField] MMFeedbacks mMFeedbackPickUp;

    [Header("Events")]
    [SerializeField] private UnityEvent OnPickUp;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
        state = PickUpState.pickedUp;
        _animator.Play("PickUp");
        mMFeedbackPickUp?.PlayFeedbacks();
        GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * 0.5f; 
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
