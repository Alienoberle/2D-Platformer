using UnityEngine;

public class Patrol : IState
{
    private EnemyAI _enemyAI;
    private EnemyMovement _enemyMovement;

    private Patrol(EnemyAI enemyAI, EnemyMovement enemyMovement)
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
