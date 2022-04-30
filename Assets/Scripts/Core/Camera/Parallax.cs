using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Parallax 
    // https://www.youtube.com/watch?v=tMXgLBwtsvI
    [SerializeField] private Transform subject;
    private Camera cam;
    private Vector3 _camLastPosition;
    private Vector3 _deltaMovement;
    private float _distanceFromSubject;
    private float _clippingPlane;
    private float _parallaxFactor;

    // Infinite scrolling background
    // https://www.youtube.com/watch?v=wBol2xzxCOU
    private float textureUnitSizeX;
    private float textureUnitSizeY;
    [SerializeField] private bool infiniteHorizontal = true;
    [SerializeField] private bool infiniteVertical = true;

    private void Start()
    {
        cam = Camera.main;
        _camLastPosition = cam.transform.position;
        CalculateParallaxFactor();
        DetermineTextureUnitSizes();
    }
    private void CalculateParallaxFactor()
    {
        _distanceFromSubject = transform.position.z - subject.position.z;
        _clippingPlane = (cam.transform.position.z + (_distanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));
        _parallaxFactor = Mathf.Abs(_distanceFromSubject) / _clippingPlane;
    }
    private void DetermineTextureUnitSizes()
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;
        var texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }
    private void LateUpdate()
    {
        _deltaMovement = cam.transform.position - _camLastPosition;
        transform.position += _deltaMovement * _parallaxFactor;
        _camLastPosition = cam.transform.position;

        if (infiniteHorizontal)
        {
            if (Mathf.Abs(cam.transform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (cam.transform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cam.transform.position.x + offsetPositionX, transform.position.y);
            }
        }
        if (infiniteVertical)
        {
            if (Mathf.Abs(cam.transform.position.y - transform.position.y) >= textureUnitSizeY)
            {
                float offsetPositionY = (cam.transform.position.y - transform.position.y) % textureUnitSizeY;
                transform.position = new Vector3(transform.position.x, cam.transform.position.y + offsetPositionY);
            }
        }
    }
}
