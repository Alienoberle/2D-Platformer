using UnityEngine;

public class Patrol : IState
{
    public string name { get { return "Patrol"; } }
    private readonly EnemyPathfinding enemyMovement;
    private readonly Animator animator;
    private Vector3 startingPosition;
    private Vector3[] waypoints;
    private Vector3[] globalWaypoints;
    private int waypointIndex;
    private bool isCyclic;

    public Patrol(EnemyPathfinding enemyMovement, Vector3 startingPosition ,Vector3[] waypoints, Animator animator)
    {
        this.enemyMovement = enemyMovement;
        this.startingPosition = startingPosition;
        this.waypoints = waypoints;
        this.animator = animator;
    }
    public void OnEnter()
    {
        animator.SetTrigger("Move");
        enemyMovement.enabled = true;
        globalWaypoints = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            globalWaypoints[i] = waypoints[i] + startingPosition;
        }
        
        waypointIndex = 0;
        enemyMovement.GoToTarget(globalWaypoints[waypointIndex]);
        enemyMovement.OnTargetReached += OnTargetReached;
    }
   public void OnTargetReached()
    {
        waypointIndex++;
        if (waypointIndex > globalWaypoints.Length - 1)
        {
            waypointIndex = 0;
            System.Array.Reverse(globalWaypoints);
        }
        enemyMovement.GoToTarget(globalWaypoints[waypointIndex]);

    }
    public void Tick()
    {

    }  
    public void OnExit()
    {
        animator.ResetTrigger("Move");
        enemyMovement.enabled = false;
        enemyMovement.OnTargetReached -= OnTargetReached;
    }
}
