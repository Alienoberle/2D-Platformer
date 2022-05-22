using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMagnetism : Magnet
{
    //Aiming 
    private Vector2 _aimInput;
    private Vector2 _lastValidAimInput = new Vector2(1, 0);
    private Vector2 _aimDirection;
    private string _controlScheme;
    private Vector2 _raycastOrigin;
    private float _rayLenght = 10.0f;
    [SerializeField] private LayerMask _layerMask;
    private HashSet<Magnet> _hitLastFrame = new HashSet<Magnet>();
    [SerializeField] private GameObject _visualisation;

    private void Update()
    {
        Aim();
        HandleAimVisualisation();
        HitDetection();
        HandleHits();
    }
    #region Aim & Hit detection
    private void Aim()
    {
        // Check aim input in case of 
        if (_aimInput == Vector2.zero)
        {
            _aimInput = _lastValidAimInput;
        }
        switch (_controlScheme)
        {
            case "Keyboard&Mouse":
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
                _aimDirection = (_aimInput - screenPoint).normalized;
                break;
            case "Gamepad":
                _aimDirection = _aimInput.normalized;
                break;
        }
        _lastValidAimInput = _aimInput;
    }
    private void HandleAimVisualisation()
    {
        _visualisation.transform.position = (Vector2)transform.position + _aimDirection * _rayLenght;
        float angle = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg;
        _visualisation.transform.rotation = Quaternion.Euler(0f, 0f, playerController.playerInfo.facingDirection > 0 ? angle : angle + 180);
    }
    private void HitDetection()
    {
        _raycastOrigin = transform.position;
        var hit = Physics2D.Raycast(_raycastOrigin, _aimDirection, _rayLenght, _layerMask);
        Debug.DrawRay(_raycastOrigin, _aimDirection * _rayLenght);
        
        affectedByMagnets.Clear();

        if (hit)
        {
            var magnet = hit.transform.GetComponent<Magnet>();
            if (magnet is null) return;
            if (!hit.transform.CompareTag("Player") && !hit.transform.CompareTag("Magnet")) return;
            if (affectedByMagnets.Contains(magnet)) return;
            affectedByMagnets.Add(magnet);
        }
        #region MultiHit - Inactive
        //var hits = Physics2D.RaycastAll(_raycastOrigin, _aimDirection, _rayLenght, _layernMask);  // if we want to have multi hits
        //if (hits.Length > 0)
        //{
        //    foreach (RaycastHit2D hitMagnet in hits)
        //    {
        //        var magnet = hitMagnet.transform.GetComponent<Magnet>();
        //        if (magnet is null) return;
        //        if (!hitMagnet.transform.CompareTag("Player") && !hitMagnet.transform.CompareTag("Magnet")) return;
        //        if (affectedByMagnets.Contains(magnet)) return;
        //        affectedByMagnets.Add(magnet);
        //    }
        //}
        #endregion
    }
    private void HandleHits()
    {
        foreach(Magnet magnet in affectedByMagnets)
        {
            if (magnet.GetType() != typeof(PlayerMagnetism))
            {
                magnet.Highlight();
            }
            magnet.affectedByMagnets.Add(this);
        }

        _hitLastFrame.ExceptWith(affectedByMagnets);
        foreach (Magnet magnet in _hitLastFrame)
        {
            if (!affectedByMagnets.Contains(magnet))
            {
                magnet.UnHighlight();
                magnet.affectedByMagnets.Remove(this);
            }
        }
        _hitLastFrame.Clear();
        _hitLastFrame.UnionWith(affectedByMagnets);
    }
    #endregion
    #region Player specific magnet functionality
    public override void ApplyMagneticForce(Vector2 velocity)
    {
        playerController.playerInfo.isMagnetismActive = (velocity != Vector2.zero) ? true : false;
        playerController.magneticVelocity = velocity;
    }
    public override void ChangePolarisation(Polarization newPolarization)
    {
        base.ChangePolarisation(newPolarization);
        switch (newPolarization)
        {
            case Polarization.negative:
                _visualisation.GetComponent<SpriteRenderer>().color = Color.red;
                spriteRenderer.material.EnableKeyword("OUTBASE_ON");
                spriteRenderer.material.SetColor("_OutlineColor", Color.red);
                break;
            case Polarization.positive:
                _visualisation.GetComponent<SpriteRenderer>().color = Color.blue;
                spriteRenderer.material.EnableKeyword("OUTBASE_ON");
                spriteRenderer.material.SetColor("_OutlineColor", Color.blue);
                break;
            case Polarization.neutral:
                spriteRenderer.material.DisableKeyword("OUTBASE_ON");
                affectedByMagnets.Clear();
                break;
        }
    }
    #endregion
    #region Helperfunctions
    public void SetAimInput(Vector2 input)
    {
        _aimInput = input;
    }
    public void SetControlScheme(string controlScheme)
    {
        _controlScheme = controlScheme;
    }
    #endregion
}
