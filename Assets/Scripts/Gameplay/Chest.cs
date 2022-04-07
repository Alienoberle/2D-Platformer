using UnityEngine;
using DG.Tweening;

public class Chest : MonoBehaviour
{
    private Animator animator;
    private bool negIsLocked = true;
    private bool posIsLocked = true;
    private bool isUnlocked = false;

    [SerializeField] private GameObject magnetNegative;
    private Collider2D col2DMagnetNegative;
    [SerializeField] private GameObject magnetPositive;
    private Collider2D colDMagnetPositive;

    [SerializeField] private AudioClip sFXLockRemoved;
    [SerializeField] private ParticleSystem vFXChestOpens;
    private const float yDistance = 2f;
    private const float Duration = 1.5f;
    [SerializeField] private AudioClip sFXChestOpens;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col2DMagnetNegative = magnetNegative.GetComponent<Collider2D>();
        colDMagnetPositive = magnetPositive.GetComponent<Collider2D>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isUnlocked) return;
        if (collision == col2DMagnetNegative)
        {
            negIsLocked = false;
            magnetNegative.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            CheckChestState();
        }
        if (collision == colDMagnetPositive)
        {
            posIsLocked = false;
            magnetPositive.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            CheckChestState();
        }
    }
    private void CheckChestState()
    {
        if(negIsLocked == false && posIsLocked == false)
        {
            isUnlocked = true;
            ChestUnlocked();
        }
        else
        {
            LockRemoved();
        }
    }
    [ContextMenu("LockRemoved")]
    private void LockRemoved()
    {
        animator.Play("LockRemoved");
        AudioManager.Instance.PlaySound(sFXLockRemoved);
    }
    [ContextMenu("ChestUnlocked")]
    private void ChestUnlocked()
    {
        animator.Play("Open");
        AudioManager.Instance.PlaySound(sFXChestOpens);
    }

    private void ItemPopOut()
    {
        vFXChestOpens.Play();
        vFXChestOpens.transform.DOLocalMoveY(yDistance, Duration);
    }
}
