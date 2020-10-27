using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
