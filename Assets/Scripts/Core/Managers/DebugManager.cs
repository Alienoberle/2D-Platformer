using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    private Controls controls;
    private bool debugMenuOpen;

    // Singleton set up
    private static DebugManager _instance;
    public static DebugManager instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<DebugManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        //playerControls = new PlayerControls();
        debugMenuOpen = false;
    }

    private void OnEnable()
    {
        //playerControls.Debug.Enable();
        //playerControls.Debug.DebugMenu.performed += context => ToggleDebugMenu();
        //playerControls.Debug.GodMode.performed += context => GodMode();
        //playerControls.Debug.Suicide.performed += context => Suicide();
    }

    private void OnDisable()
    {
        //playerControls.Debug.Disable();
        //playerControls.Debug.DebugMenu.performed -= context => ToggleDebugMenu();
        //playerControls.Debug.GodMode.performed -= context => GodMode();
        //playerControls.Debug.Suicide.performed -= context => Suicide();
    }

    private void ToggleDebugMenu()
    {
        if(debugMenuOpen == false)
        {
            SceneManager.LoadSceneAsync("DebugMenu", LoadSceneMode.Additive);
            //FindObjectOfType<PlayerInputHandler>().DisablePlayerControls();
            Time.timeScale = 0.0f;
            debugMenuOpen = true;
        }
        else
        {
            SceneManager.UnloadSceneAsync("DebugMenu");
            //FindObjectOfType<PlayerInputHandler>().EnablePlayerControls();
            Time.timeScale = 1.0f;
            debugMenuOpen = false;
        }
    }
    private void GodMode()
    {

    }
    private void Suicide()
    {

    }
}
