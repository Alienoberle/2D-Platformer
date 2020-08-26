using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;


[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    private InputManager inputManager;
    private Player player;
    private PlayerController playerController;

    public Vector2 directionalInput;

    private void Awake()
    {
        inputManager = new InputManager();
        player = GetComponent<Player>();
        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        inputManager.Player.Enable();
        inputManager.Player.Movement.performed += Movement;
        inputManager.Player.Jump.performed += Jump;
        inputManager.Player.JumpRelease.performed += JumpRelease;
        inputManager.Player.Dash.performed += Dash;
    }

    private void OnDisable()
    {
        inputManager.Player.Disable();
        inputManager.Player.Movement.performed -= Movement;
        inputManager.Player.Jump.performed -= Jump;
        inputManager.Player.JumpRelease.performed -= JumpRelease;
        inputManager.Player.Dash.performed -= Dash;
    }

    private void Movement(InputAction.CallbackContext context)
    {
        directionalInput = inputManager.Player.Movement.ReadValue<Vector2>();
        player.SetDirectionalInput(directionalInput);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        player.OnJumpInput();
    }

    private void JumpRelease(InputAction.CallbackContext context)
    {
        player.OnJumpInputRelease();
    }

    private void Dash(InputAction.CallbackContext context)
    {
        player.Dash();
    }

}
