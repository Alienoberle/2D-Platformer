using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    // Singleton set up
    private static Player _instance;
    public static Player instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Player>();
            if (_instance == null)
            {
                Debug.Log("No player found");
            }
            return _instance;
        }
    }

    public bool isAlive { get; set; }
    public Health playerHealth;

    private void OnEnable()
    {
        playerHealth.OnHealthZero += HandleOnHealthZero;
        playerHealth.OnHealthChanged += HandleOnHealthChanged;
    }

    private void HandleOnHealthChanged(int arg1, int arg2)
    {
        throw new System.NotImplementedException();
    }

    private void HandleOnHealthZero(ScriptableObject obj)
    {
        //LevelLoader.instance;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        playerHealth.OnHealthZero -= HandleOnHealthZero;
        playerHealth.OnHealthChanged -= HandleOnHealthChanged;
    }
}
