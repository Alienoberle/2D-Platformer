using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUp : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider2D _collider;
    private float _destructionDelay = 1.5f;
    public bool isMoving { get; private set; }
    public bool isPickedUp { get; private set; }  

    [SerializeField] private UnityEvent OnPickUp;

    private void Awake()
    {
        _animator.Play("Idle");
    }
    private void Moving()
    {
        isMoving = true;
        _animator.Play("MoveToPlayer");
    }
    private void HandlePickUp()
    {
        isPickedUp = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        _animator.Play("PickUp");
    }
    // Triggerd through Animation Event in "PickUp" animation
    private void PickedUp()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, _destructionDelay);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _collider.enabled = false;
            HandlePickUp();
            OnPickUp.Invoke();
        } 
    }
}
