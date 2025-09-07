using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest : MonoBehaviour
{
    public Collider2D areaCollider;
    private int numOverlaps;
    private ContactFilter2D filter = new ContactFilter2D();
    private List<Collider2D> overlaps = new List<Collider2D>();

    private void Start()
    {
        filter.SetLayerMask(LayerMask.GetMask("Magnet"));
    }

    // Update is called once per frame
    void Update()
    {

        numOverlaps = areaCollider.Overlap(filter, overlaps);
        if(numOverlaps > 5)
        {
            Debug.Log("Overlaps: " + numOverlaps);
            foreach (Collider2D overlap in overlaps)
            {
                Debug.Log(overlap.gameObject.name);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enter: " + collision.gameObject.name);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Stay: " + collision.gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit: " + collision.gameObject.name);
    }
}
