using UnityEngine;

public class Patrol : IState
{
    public string name { get { return "Patrol"; } }
    private EnemyPathfinding _enemyMovement;

    private Vector3 _startingPosition;
    private Vector3[] _waypoints;
    private Vector3[] _globalWaypoints;
    private int waypointIndex;
    private bool isCyclic;

    public Patrol(EnemyPathfinding enemyMovement, Vector3 startingPosition ,Vector3[] waypoints )
    {
        _startingPosition = startingPosition;
        _enemyMovement = enemyMovement;
        _waypoints = waypoints;
    }
    public void OnEnter()
    {
        _globalWaypoints = new Vector3[_waypoints.Length];
        for (int i = 0; i < _waypoints.Length; i++)
        {
            _globalWaypoints[i] = _waypoints[i] + _startingPosition;
        }
        
        waypointIndex = 0;
        _enemyMovement.GoToTarget(_globalWaypoints[waypointIndex]);
        _enemyMovement.OnTargetReached += OnTargetReached;
    }
   public void OnTargetReached()
    {
        waypointIndex++;
        if (waypointIndex > _globalWaypoints.Length - 1)
        {
            waypointIndex = 0;
            System.Array.Reverse(_globalWaypoints);
        }
        _enemyMovement.GoToTarget(_globalWaypoints[waypointIndex]);

    }
    public void Tick()
    {

    }  
    public void OnExit()
    {
        _enemyMovement.OnTargetReached -= OnTargetReached;
    }
}
