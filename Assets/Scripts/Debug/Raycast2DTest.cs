using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast2DTest : MonoBehaviour

{
    public Collider2D colider2D;
    public int numHits;
    public float distance = 10.0f;
    public ContactFilter2D filter;
    public RaycastHit2D[] hits = new RaycastHit2D[10];
    public RaycastHit2D[] hits2 = new RaycastHit2D[10];

    void Update()
    {
        numHits = colider2D.Raycast(Vector2.right, filter, hits, distance);
        Debug.DrawRay(transform.position, new Vector2(distance, 0));

        if (numHits > 0)
        {
            foreach(RaycastHit2D hit in hits)
            {
                Debug.Log("hit " + hit.transform.gameObject + " at position " + hit.point);
                Debug.DrawRay(hit.point, new Vector2(0.5f, 0));
                Debug.DrawRay(hit.point, new Vector2(-0.5f, 0));
                Debug.DrawRay(hit.point, new Vector2(0, 0.5f));
                Debug.DrawRay(hit.point, new Vector2(0, -0.5f));
            }
        }

        int numhits2 = Physics2D.Raycast(transform.position, Vector2.left, filter, hits2);
        Debug.DrawRay(transform.position, new Vector2(-distance, 0), Color.red);
       
        if (numhits2 > 0)
        {
            foreach (RaycastHit2D hit in hits2)
            {
                Debug.Log("hit " + hit.transform.gameObject + " at position " + hit.point);
                Debug.DrawRay(hit.point, new Vector2(0.5f, 0));
                Debug.DrawRay(hit.point, new Vector2(-0.5f, 0));
                Debug.DrawRay(hit.point, new Vector2(0, 0.5f));
                Debug.DrawRay(hit.point, new Vector2(0, -0.5f));
            }
        }

    }
}
