using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnetism : Magnet
{
    private Camera mainCamera;
    private Vector2 raycastOrigin;
    public Vector2 aimInput { get; private set; }
    public float rayLenght { get; private set; }
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private GameObject visualisation;

   private void Awake()
    {
        mainCamera = Camera.main;
        rayLenght = 10f;
    }

    private void Update()
    {
       Aim();
    }

    private void Aim()
    {
        Vector2 screenPoint = mainCamera.WorldToScreenPoint(transform.position);
        Vector2 aimDirection = (aimInput - screenPoint).normalized;

        raycastOrigin = transform.position;
        var hit = Physics2D.RaycastAll(raycastOrigin, aimDirection, rayLenght, collisionMask);
        Debug.DrawRay(raycastOrigin, aimDirection * rayLenght);

        if (hit.Length > 0)
        {
            foreach(RaycastHit2D hitMagnet in hit)
            {
                Debug.Log(hitMagnet.transform.name);
            }
        }

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
