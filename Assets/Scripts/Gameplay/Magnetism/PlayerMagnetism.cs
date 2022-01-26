using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMagnetism : Magnet
{
    private SpriteRenderer spriteRenderer;
    private Material material;
    public Vector2 aimInput { get; private set; }
    private Vector2 aimDirection;
    private Vector2 raycastOrigin;
    private float rayLenght = 10f;
    [SerializeField] private LayerMask collisionMask;

    private HashSet<Magnet> hitLastFrame = new HashSet<Magnet>();
    [SerializeField] private GameObject visualisation;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }
    private void Update()
    {
        if (currentPolarization != Polarization.neutral)
        {
            Aim();
            HitDetection();
            RotateVisualisation();
        }
    }
    private void Aim()
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        aimDirection = (aimInput - screenPoint).normalized;
    }

    private void HitDetection()
    {
        raycastOrigin = transform.position;
        var hits = Physics2D.RaycastAll(raycastOrigin, aimDirection, rayLenght, collisionMask);
        Debug.DrawRay(raycastOrigin, aimDirection * rayLenght);

        if (hits.Length > 0)
        {
            inRangeOfMagnets.Clear();
            foreach (RaycastHit2D hitMagnet in hits)
            {
                if (hitMagnet.transform.CompareTag("Magnet"))
                {
                    if (!inRangeOfMagnets.Contains(hitMagnet.transform.GetComponent<Magnet>()))
                    {
                        inRangeOfMagnets.Add(hitMagnet.transform.GetComponent<Magnet>());
                    }
                }
            }
        }

        hitLastFrame.ExceptWith(inRangeOfMagnets);
        foreach (Magnet magnet in hitLastFrame)
        {
            if (!inRangeOfMagnets.Contains(magnet))
            {
                print("This magnet " + magnet + " is no longer aimed at!");
            }
        }
        hitLastFrame.Clear();
        hitLastFrame.UnionWith(inRangeOfMagnets);
    }
    private void RotateVisualisation()
    {
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        visualisation.transform.rotation = Quaternion.Euler(0f, 0f, playerController.playerInfo.facingDirection > 0 ? angle : angle + 180);
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
                material.EnableKeyword("OUTBASE_ON");
                material.SetColor("_OutlineColor", Color.red);
                break;
            case Polarization.positive:
                visualisation.SetActive(true);
                visualisation.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                material.EnableKeyword("OUTBASE_ON");
                material.SetColor("_OutlineColor", Color.blue);
                break;
            case Polarization.neutral:
                visualisation.SetActive(false);
                material.DisableKeyword("OUTBASE_ON");
                break;
        }
    }
}
