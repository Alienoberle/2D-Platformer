using UnityEngine;

public class Idle : IState
{
    public string name {get {return "Idle";}}
    private EnemyAI _enemyAI;
    private EnemyMovement _enemyMovement;

    public Idle(EnemyAI enemyAI, EnemyMovement enemyMovement)
    {
        _enemyAI = enemyAI;
        _enemyMovement = enemyMovement;
    }
    public void Tick()
    {
        Debug.Log("Idle Tick");
    }
    public void OnEnter()
    {
        Debug.Log("Idle Enter");
    }
    public void OnExit()
    {
        Debug.Log("Idle Exit");
    }
}
