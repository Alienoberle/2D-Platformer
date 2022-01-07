using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerMagnetism))]
public class PlayerInputHandler : MonoBehaviour
{
    //private Controls controls;
    private PlayerController playerController;
    private PlayerMagnetism playerMagnetism;
    public Vector2 directionalInput { get; private set; }
    public Vector2 aimInput { get; private set; }

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerMagnetism = GetComponent<PlayerMagnetism>();
    }

    public void Movement(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) // or if (!gameObject.scene.IsValid()) see: https://stackoverflow.com/questions/62707625/unity-input-system-button-triggers-multiple-times
        {
            return;
        }
        directionalInput = context.ReadValue<Vector2>();
        playerController.SetDirectionalInput(directionalInput);
    }
    public void Aim(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) // or if (!gameObject.scene.IsValid()) see: https://stackoverflow.com/questions/62707625/unity-input-system-button-triggers-multiple-times
        {
            return;
        }
        aimInput = context.ReadValue<Vector2>();
        playerMagnetism.SetAimInput(aimInput);
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) // or if (!gameObject.scene.IsValid())
        {
            return;
        }
        if (context.performed)
        {
            playerController.OnJumpInput();
        }
        if (context.canceled)
        {
            playerController.OnJumpInputRelease();
        }
    }
    public void Dash(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) // or if (!gameObject.scene.IsValid())
        {
            return;
        }

        if (context.performed)
        {
            playerController.OnDashInput();
        }
    }
    public void ChargePositive(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) // or if (!gameObject.scene.IsValid())
        {
            return;
        }
        if (context.performed)
        {
            playerMagnetism.OnChangeCharge(Polarization.positive);
            playerController.OnChangeCharge(Polarization.positive);
        }
        else if (context.canceled)
        {
            playerMagnetism.OnChangeCharge(Polarization.neutral);
            playerController.OnChangeCharge(Polarization.neutral);
        }
    }
    public void ChargeNegative(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) // or if (!gameObject.scene.IsValid())
        {
            return;
        }
        if (context.performed)
        {
            playerMagnetism.OnChangeCharge(Polarization.negative);
            playerController.OnChangeCharge(Polarization.negative);
        }
        else if (context.canceled)
        {
            playerMagnetism.OnChangeCharge(Polarization.neutral);
            playerController.OnChangeCharge(Polarization.neutral);
        }
    }
}
