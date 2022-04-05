using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool negIsLocked = true;
    private bool posIsLocked = true;
    private bool isUnlocked = false;

    [SerializeField] private GameObject magnetNegative;
    private Collider2D col2DMagnetNegative;
    [SerializeField] private GameObject magnetPositive;
    private Collider2D colDMagnetPositive;

    [SerializeField] private ParticleSystem vFXChestUnlocked;
    [SerializeField] private AudioClip sFXChestUnlocked;

    private void Awake()
    {
        col2DMagnetNegative = magnetNegative.GetComponent<Collider2D>();
        colDMagnetPositive = magnetPositive.GetComponent<Collider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
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
            print("unlocked");
        }
    }

    private void ChestUnlocked()
    {
        vFXChestUnlocked.Play();
        AudioManager.Instance.PlaySound(sFXChestUnlocked);
    }
}
