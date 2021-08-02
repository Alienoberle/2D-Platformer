using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 startPosition;
    public List<Transform> cameraTargets;
    private Bounds debugBounds;

    private void OnEnable()
    {
        if (startPosition == null)
            startPosition = Vector3.zero;
    }
    private void LateUpdate()
    {
        transform.position = GetCenterPoint();
    }

    private Vector3 GetCenterPoint()
    {
        if (cameraTargets.Count < 1)
            return startPosition;
        else if (cameraTargets.Count == 1)
            return cameraTargets[0].position;
        else 
        {
            var bounds = new Bounds(cameraTargets[0].position, Vector2.zero);
            for (int i = 0; i < cameraTargets.Count; i++)
            {
                bounds.Encapsulate(cameraTargets[i].position);
            }
            debugBounds = bounds;
            return bounds.center;
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
