using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Chest : MonoBehaviour
{
    private Animator animator;
    private AudioSource audiosource;
    private bool negIsLocked = true;
    private bool posIsLocked = true;
    private bool isUnlocked = false;

    [Header("Locking Magnets")]
    [SerializeField] private GameObject magnetNegative;
    private Collider2D col2DMagnetNegative;
    [SerializeField] private GameObject magnetPositive;
    private Collider2D colDMagnetPositive;

    [Header("Feedback")]
    [SerializeField] private AudioClip sFXLockRemoved;
    [SerializeField] private AudioClip sFXChestUnlocked;
    [SerializeField] private ParticleSystem vFXItemAppearance;
    [SerializeField] private AudioClip sFXItemAppearance;

    [Header("Chest Content")]

    [SerializeField] private GameObject lootItem;
    [SerializeField] private float yDistance = 2f;
    [SerializeField] private float Duration = 1.5f;

    [Header("Events")]
    [SerializeField] private UnityEvent OnLockRemoved;
    [SerializeField] private UnityEvent OnChestOpen;
    [SerializeField] private UnityEvent ItemAppeared;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
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
        audiosource.PlayOneShot(sFXLockRemoved);
        OnLockRemoved.Invoke();
    }
    [ContextMenu("ChestUnlocked")]
    private void ChestUnlocked()
    {
        animator.Play("Open");
        audiosource.PlayOneShot(sFXChestUnlocked);
        OnChestOpen.Invoke();
    }

    // Called from the "ChestOpen" Animation
    private void ItemPopOut()
    {
        vFXItemAppearance.Play();
        audiosource.PlayOneShot(sFXItemAppearance);
        var loot = Instantiate(lootItem, transform.position, Quaternion.identity);
        loot.transform.DOLocalMoveY(yDistance, Duration).SetRelative().OnComplete(() => ItemAppeared.Invoke());
    }
}
