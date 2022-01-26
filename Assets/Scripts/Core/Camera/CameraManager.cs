using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraManager : StaticInstance<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject startPosition;
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private float minOrtographicSize = 10.0f; // max. zoom
    [SerializeField] private float maxOrtographicSize = 15.0f; // min. zoom
    [SerializeField] private float zoomLimit = 30.0f;
    public List<Transform> cameraTargets;
    private Bounds debugBounds;

    private void OnEnable()
    {
        Move(startPosition.transform.position);
    }
    private void LateUpdate()
    {
        if (cameraTargets.Count < 1)
            return;
        (Vector3 center, float sizeX) cameraBounds = CalculateCameraBounds();
        Move(cameraBounds.Item1);
        Zoom(cameraBounds.Item2);
    }

    private void Move(Vector3 center)
    {
        cameraTarget.transform.position = center;
    }
    private void Zoom(float sizeX)
    {
        float newOrtographicSize = Mathf.Lerp(minOrtographicSize, maxOrtographicSize, Mathf.Clamp01(sizeX / zoomLimit));
        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, newOrtographicSize, Time.deltaTime);
       ;
    }

    private (Vector3 center, float sizeX) CalculateCameraBounds()
    {
        if (cameraTargets.Count == 1)
            return (cameraTargets[0].position, 0);
        else 
        {
            var bounds = new Bounds(cameraTargets[0].position, Vector2.zero);
            for (int i = 0; i < cameraTargets.Count; i++)
            {
                bounds.Encapsulate(cameraTargets[i].position);
            }
            debugBounds = bounds;
            return (bounds.center, bounds.size.x);
        }
    }
    public void AddCameraTarget(PlayerInput input) => cameraTargets.Add(input.gameObject.transform);
    public void RemoveCameraTarget(PlayerInput input) => cameraTargets.Remove(input.gameObject.transform);

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(debugBounds.center, 1.0f);
            Gizmos.DrawWireCube(debugBounds.center, debugBounds.size);
        }
    }
}
