using UnityEngine;
public class Chase : IState
{
    public string name { get { return "Chase"; } }
    private readonly EnemyPathfinding enemyMovement;
    private readonly GameObject target;
    private readonly Animator animator;
    private float repathRate = 0.5f;
    private float repathCounter = 0;
    public float timeStuck { get; private set; }
    private Vector3 lastPosition = Vector3.zero;

    public Chase(EnemyPathfinding enemyMovement, GameObject target, Animator animator)
    {
        this.enemyMovement = enemyMovement;
        this.target = target;
        this.animator = animator;
    }
    public void OnEnter()
    {
        animator.SetTrigger("Move");
        enemyMovement.enabled = true;
        enemyMovement.GoToTarget(target.transform.position);
        enemyMovement.OnTargetReached += OnTargetReached;
        timeStuck = 0.0f;
        repathCounter = repathRate;
    }
    public void Tick()
    {
        repathCounter -= Time.deltaTime;
        if (repathCounter < 0)
        {
            enemyMovement.GoToTarget(target.transform.position);
            repathCounter = repathRate;
        }

        if (Vector3.Distance(target.transform.position, lastPosition) <= 0f)
            timeStuck += Time.deltaTime;

        lastPosition = target.transform.position;
    }
    public void OnTargetReached()
    {
        Debug.Log("Target Caught");
    }
    public void OnExit()
    {
        animator.ResetTrigger("Move");
        enemyMovement.enabled = false;
        enemyMovement.OnTargetReached -= OnTargetReached;
    }
}
