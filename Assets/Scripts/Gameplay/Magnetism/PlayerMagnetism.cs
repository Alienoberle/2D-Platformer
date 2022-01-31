using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMagnetism : Magnet
{
    public Vector2 aimInput { get; private set; }
    private Vector2 lastValidAimInput;
    private Vector2 aimDirection;
    public string controlScheme;
    private Vector2 raycastOrigin;
    private float rayLenght = 10.0f;
    [SerializeField] private LayerMask collisionMask;
    private HashSet<Magnet> hitLastFrame = new HashSet<Magnet>();

    [SerializeField] private GameObject visualisation;
    private Material material;

    private void Start()
    {
        material = spriteRenderer.material;
        lastValidAimInput = new Vector2(1, 0);
    }
    private void Update()
    {
        Aim(); 
        HitDetection();
        HandleVisualisation();
        HandleAimHighlighting();
    }

    private void Aim()
    {
        // Check aim input in case of 
        if (aimInput == Vector2.zero)
        {
            aimInput = lastValidAimInput;
        }
        switch (controlScheme)
        {
            case "Keyboard&Mouse":
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
                aimDirection = (aimInput - screenPoint).normalized;
                break;
            case "Gamepad":
                aimDirection = aimInput.normalized;
                break;
        }

        lastValidAimInput = aimInput;
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
                    var magnet = hitMagnet.transform.GetComponent<Magnet>();
                    if (!inRangeOfMagnets.Contains(magnet))
                    {
                        inRangeOfMagnets.Add(magnet);
                    }
                }
            }
        }
    }

    private void HandleAimHighlighting()
    {
        foreach(Magnet magnet in inRangeOfMagnets)
        {
            magnet.Highlight();
        }

        hitLastFrame.ExceptWith(inRangeOfMagnets);
        foreach (Magnet magnet in hitLastFrame)
        {
            if (!inRangeOfMagnets.Contains(magnet))
            {
                magnet.UnHighlight();
            }
        }
        hitLastFrame.Clear();
        hitLastFrame.UnionWith(inRangeOfMagnets);
    }

    private void HandleVisualisation()
    {
        visualisation.transform.position = (Vector2)transform.position + aimDirection * rayLenght;
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
                visualisation.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                material.EnableKeyword("OUTBASE_ON");
                material.SetColor("_OutlineColor", Color.red);
                break;
            case Polarization.positive:
                visualisation.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                material.EnableKeyword("OUTBASE_ON");
                material.SetColor("_OutlineColor", Color.blue);
                break;
            case Polarization.neutral:
                material.DisableKeyword("OUTBASE_ON");
                inRangeOfMagnets.Clear();
                break;
        }
    }
}
