using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnetism : Magnet
{
    public Vector2 aimInput { get; private set; }
    private Vector2 raycastOrigin;
    private float rayLenght = 10f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private GameObject visualisation;

    private void Update()
    {
        if (currentPolarization != Polarization.neutral)
        {
            Aim();
        }
    }

    private void Aim()
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 aimDirection = (aimInput - screenPoint).normalized;

        raycastOrigin = transform.position;
        var hits = Physics2D.RaycastAll(raycastOrigin, aimDirection, rayLenght, collisionMask);
        Debug.DrawRay(raycastOrigin, aimDirection * rayLenght);

        if (hits.Length <= 0)
        {
            magnetsInRange.Clear();
        }

        if (hits.Length > 0)
        {
            foreach(RaycastHit2D hitMagnet in hits)
            {
                if (hitMagnet.transform.CompareTag("Magnet"))
                {
                    if (!magnetsInRange.Contains(hitMagnet.transform.GetComponent<Magnet>()))
                    {
                        magnetsInRange.Add(hitMagnet.transform.GetComponent<Magnet>());
                        hitMagnet.transform.GetComponentInParent<Magnet>().inRangeOfMagnets.Add(this);
                    }
                }
            }
        }

        foreach(Magnet magnet in magnetsInRange)
        {
            Debug.Log(magnet.name);
        }

        foreach (Magnet magnet in inRangeOfMagnets)
        {
            Debug.Log(magnet.name);
        }

        Debug.Log("Magnets in Range: " + magnetsInRange.Count);
        Debug.Log("In Range of Magnets: " + inRangeOfMagnets.Count);

        // To Do: fix issue with flipped player character due to walking
        //to rotate smth. in the direction of the cursor
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        visualisation.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void SetAimInput(Vector2 input)
    {
        aimInput = input;
    }

    public void OnChangeCharge(Polarization newPolarization)
    {
        ChangePolarisation(newPolarization);
        switch (newPolarization)
        {
            case Polarization.negative:
                visualisation.SetActive(true);
                visualisation.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                break;
            case Polarization.positive:
                visualisation.SetActive(true);
                visualisation.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                break;
            case Polarization.neutral:
                visualisation.SetActive(false);
                break;
        }
    }
}
