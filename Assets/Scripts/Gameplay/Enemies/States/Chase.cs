using UnityEngine;
public class Chase : IState
{
    public string name { get { return "Chase"; } }
    private EnemyPathfinding enemyMovement;
    private GameObject target;
    private float repathRate = 0.5f;
    private float repathCounter = 0;

    public Chase( EnemyPathfinding enemyMovement, GameObject target)
    {
        this.enemyMovement = enemyMovement;
        this.target = target;
    }
    public void OnEnter()
    {
        repathCounter = repathRate;
        enemyMovement.GoToTarget(target.transform.position);
        enemyMovement.OnTargetReached += OnTargetReached;
    }
    public void Tick()
    {
        repathCounter -= Time.deltaTime;
        if (repathCounter < 0)
        {
            enemyMovement.GoToTarget(target.transform.position);
            repathCounter = repathRate;
        }
    }
    public void OnTargetReached()
    {
        Debug.Log("Player Caught");
    }
    public void OnExit()
    {
        enemyMovement.OnTargetReached -= OnTargetReached;
    }
}
