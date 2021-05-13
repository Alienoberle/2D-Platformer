using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerCollision))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls playerControls;
    private PlayerController playerController;
    private PlayerCollision playerCollision;

    public Vector2 directionalInput { get; private set; } 

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerController = GetComponent<PlayerController>();
        playerCollision = GetComponent<PlayerCollision>();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Movement.performed += Movement;
        playerControls.Player.Jump.performed += Jump;
        playerControls.Player.JumpRelease.performed += JumpRelease;
        playerControls.Player.Dash.performed += Dash;
        playerControls.Player.ChargePos.performed += ChargePositive;
        playerControls.Player.ChargePos.canceled += ChargePositive;
        playerControls.Player.ChargeNeg.performed += ChargeNegative;
        playerControls.Player.ChargeNeg.canceled += ChargeNegative;
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
        playerControls.Player.Movement.performed -= Movement;
        playerControls.Player.Jump.performed -= Jump;
        playerControls.Player.JumpRelease.performed -= JumpRelease;
        playerControls.Player.Dash.performed -= Dash;
        playerControls.Player.ChargePos.performed -= ChargePositive;
        playerControls.Player.ChargePos.canceled -= ChargePositive;
        playerControls.Player.ChargeNeg.performed -= ChargeNegative;
        playerControls.Player.ChargeNeg.canceled -= ChargeNegative;
    }

    private void Movement(InputAction.CallbackContext context)
    {
        directionalInput = context.ReadValue<Vector2>();
        playerController.SetDirectionalInput(directionalInput);

        if (directionalInput.y < -0.9)
        {
            playerCollision.playerPressedDown = true;
        }
        else playerCollision.playerPressedDown = false;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        playerController.OnJumpInput();
    }

    private void JumpRelease(InputAction.CallbackContext context)
    {
        playerController.OnJumpInputRelease();
    }

    private void Dash(InputAction.CallbackContext context)
    {
        playerController.OnDashInput();
    }

    private void ChargePositive(InputAction.CallbackContext context)
    {
        if (context.performed)
            playerController.OnChangeCharge(Polarization.positive);
        else
            playerController.OnChangeCharge(Polarization.neutral);
    }
    private void ChargeNegative(InputAction.CallbackContext context)
    {
        if (context.performed)
            playerController.OnChangeCharge(Polarization.negative);
        else
            playerController.OnChangeCharge(Polarization.neutral);
    }

    public void EnablePlayerControls()
    {
        playerControls.Player.Enable();
    }
    public void DisablePlayerControls()
    {
        playerControls.Player.Disable();
    }

    

}
