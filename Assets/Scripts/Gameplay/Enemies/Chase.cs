using UnityEngine;

public class Chase : IState
{
    public string name { get { return "Chase"; } }
    private EnemyAI _enemyAI;
    private EnemyMovement _enemyMovement;
    private Transform _target;
    private float _repathRate = 0.5f;

    public Chase(EnemyAI enemyAI, EnemyMovement enemyMovement, Transform target)
    {
        _enemyAI = enemyAI;
        _enemyMovement = enemyMovement;
        _target = target;
    }
    public void OnEnter()
    {
        _enemyMovement.ChaseTarget(_target, _repathRate) ;
        _enemyMovement.OnTargetReached += OnTargetReached;
    }
    public void Tick()
    {
        Debug.Log("Chase Tick");
    }
    public void OnTargetReached()
    {
        Debug.Log("Player Caught");
    }
    public void OnExit()
    {
        _enemyMovement.OnTargetReached -= OnTargetReached;
    }
}
