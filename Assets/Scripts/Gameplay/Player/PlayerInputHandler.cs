using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls playerControls;
    private PlayerMovement playerMovement;
    private PlayerController playerController;

    public Vector2 directionalInput { get; private set; } 

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerMovement = GetComponent<PlayerMovement>();
        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Movement.performed += Movement;
        playerControls.Player.Jump.performed += Jump;
        playerControls.Player.JumpRelease.performed += JumpRelease;
        playerControls.Player.Dash.performed += Dash; 
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
        playerControls.Player.Movement.performed -= Movement;
        playerControls.Player.Jump.performed -= Jump;
        playerControls.Player.JumpRelease.performed -= JumpRelease;
        playerControls.Player.Dash.performed -= Dash;
    }

    private void Movement(InputAction.CallbackContext context)
    {
        directionalInput = context.ReadValue<Vector2>();
        playerMovement.SetDirectionalInput(directionalInput);

        if (directionalInput.y < -0.9)
        {
            playerController.playerPressedDown = true;
        }
        else playerController.playerPressedDown = false;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        playerMovement.OnJumpInput();
    }

    private void JumpRelease(InputAction.CallbackContext context)
    {
        playerMovement.OnJumpInputRelease();
    }

    private void Dash(InputAction.CallbackContext context)
    {
        playerMovement.OnDashInput();
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
