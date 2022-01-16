using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnetism : Magnet
{
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private Material material;
    public Vector2 aimInput { get; private set; }
    private Vector2 raycastOrigin;
    private float rayLenght = 10f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private GameObject visualisation;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }
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

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        visualisation.transform.rotation = Quaternion.Euler(0f, 0f, playerController.playerInfo.facingDirection > 0 ? angle : angle + 180); ;
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
