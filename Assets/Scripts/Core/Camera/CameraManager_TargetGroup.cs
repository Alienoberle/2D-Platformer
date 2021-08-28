using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CameraManager_TargetGroup : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup targetGroup;

    public void AddCameraTarget(PlayerInput input)
    {
        if (targetGroup.IsEmpty)
            targetGroup.AddMember(input.gameObject.transform, 1, 6);
        else
        {
            targetGroup.AddMember(input.gameObject.transform, 0, 6);
            TweenWeight();
        }

    }
    public void RemoveCameraTarget(PlayerInput input) => targetGroup.RemoveMember(input.gameObject.transform);

    public void TweenWeight()
    {
        DOTween.To(() => targetGroup.m_Targets[1].weight, x => targetGroup.m_Targets[1].weight = x, 1, 1);
    }
}
