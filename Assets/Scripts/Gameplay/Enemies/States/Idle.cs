using UnityEngine;

public class Idle : IState
{
    public string name {get {return "Idle";}}
    private EnemyAI _enemyAI;
    private EnemyPathfinding _enemyMovement;

    public Idle(EnemyAI enemyAI, EnemyPathfinding enemyMovement)
    {
        _enemyAI = enemyAI;
        _enemyMovement = enemyMovement;
    }
    public void Tick()
    {
    }
    public void OnEnter()
    {
    }
    public void OnExit()
    {
    }
}
