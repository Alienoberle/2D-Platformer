using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerCollision))]
public class PlayerInputHandler : MonoBehaviour
{
    //private Controls controls;
    private PlayerController playerController;
    private PlayerCollision playerCollision;

    public Vector2 directionalInput { get; private set; } 

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerCollision = GetComponent<PlayerCollision>();
    }

    public void Movement(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) // or if (!gameObject.scene.IsValid()) see: https://stackoverflow.com/questions/62707625/unity-input-system-button-triggers-multiple-times
        {
            return;
        }
        directionalInput = context.ReadValue<Vector2>();
        playerController.SetDirectionalInput(directionalInput);

        if (directionalInput.y < -0.9)
        {
            playerCollision.playerPressedDown = true;
        }
        else 
            playerCollision.playerPressedDown = false;
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
            playerController.OnChangeCharge(Polarization.positive);
        else if(context.canceled)
            playerController.OnChangeCharge(Polarization.neutral);
    }
    public void ChargeNegative(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) // or if (!gameObject.scene.IsValid())
        {
            return;
        }
        if (context.performed)
            playerController.OnChangeCharge(Polarization.negative);
        else if (context.canceled)
            playerController.OnChangeCharge(Polarization.neutral);
    }
}
