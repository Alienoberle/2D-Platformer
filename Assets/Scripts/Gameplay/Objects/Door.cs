using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{

    private Animator animator;
    private AudioSource audiosource;
    public DoorState _doorState { get; private set; }

    [Header("Set Up")]
    [SerializeField] private DoorState startingState = DoorState.Closed;

    [Header("Feedback")]
    [SerializeField] private AudioClip sFXOpen;
    [SerializeField] private ParticleSystem vFXOpen;
    [SerializeField] private AudioClip sFXClose;
    [SerializeField] private ParticleSystem vFXClose;
    [SerializeField] private AudioClip sFXUnlock;
    [SerializeField] private ParticleSystem vFXUnlock;

    [Header("Events")]
    [SerializeField] private UnityEvent DoorOpened;
    [SerializeField] private UnityEvent DoorClosed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        _doorState = startingState;
        switch (_doorState)
        {
            case DoorState.Locked:
                animator.Play("Closed");
                break;
            case DoorState.Unlocked:
                animator.Play("Closed");
                break;
            case DoorState.Closed:
                animator.Play("Closed");
                break;
            case DoorState.Open:
                animator.Play("Open");
                break;
        }
    }
    [ContextMenu("OpenDoor")]
    public void OpenDoor()
    {
        if (_doorState == DoorState.Open) return;
        animator.Play("Opening");
        audiosource.PlayOneShot(sFXOpen);
        vFXOpen.Play();
        _doorState = DoorState.Open;
    }
    [ContextMenu("CloseDoor")]
    public void CloseDoor()
    {
        if (_doorState == DoorState.Closed || _doorState == DoorState.Locked) return;
        animator.Play("Closeing");
        audiosource.PlayOneShot(sFXClose);
        vFXClose.Play();
        _doorState = DoorState.Closed;
    }
    public void UnlockDoor()
    {
        audiosource.PlayOneShot(sFXUnlock);
        vFXUnlock.Play();
    }
    public enum DoorState { Closed, Open, Locked, Unlocked}
}


