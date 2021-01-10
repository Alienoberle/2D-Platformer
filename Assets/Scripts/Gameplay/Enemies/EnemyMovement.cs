using System;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement : MonoBehaviour
{
    EnemyController enemyController;

    private Transform chaseTarget;
    private Vector3 targetLocation;
    private Vector3 velocity;
    public bool active { get; private set; }

    [Header("Movement")]
    public float movementSpeed;
    public float maximumSlopeAngle;
    [Header("Pathfinding")]
    public float nextWaypointDistance = 0.1f;

    private Seeker seeker;
    private Path currentPath;
    private int currentWaypoint = 0;
    private bool reachedEnd = false;

    public event Action OnTargetReached = delegate { };

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        enemyController.maxSlopeAngle = maximumSlopeAngle;
        seeker = GetComponent<Seeker>();
    }
    public void GoToTarget(Vector3 target)
    {
        targetLocation = target;
        UpdatePath();
        active = true;
    }
    public void ChaseTarget(Transform target, float repathRate)
    {
        chaseTarget = target;
        InvokeRepeating("UpdatePath", repathRate, repathRate);
        active = true;
    }
    private void OnPathCalculationComplete(Path path)
    {
        if (!path.error)
        {
            currentPath = path;
            currentWaypoint = 0;
        }
    }
    private void UpdatePath()
    {
        if (chaseTarget != null)
        {
            targetLocation = chaseTarget.position;
        }
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, targetLocation, OnPathCalculationComplete);
        }
    }
    private void Update()
    {
        if (active)
        {
            if (currentPath == null)
            {
                return;
            }

            reachedEnd = false;
            float distanceToWaypoint;
            while (true)
            {
                distanceToWaypoint = Vector3.Distance(transform.position, currentPath.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < nextWaypointDistance)
                {
                    if (currentWaypoint + 1 < currentPath.vectorPath.Count)
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        reachedEnd = true;
                        active = false;
                        OnTargetReached();
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            CalculateVelocity();
            enemyController.Move(velocity * Time.deltaTime, false);
        }      
    }

    private void CalculateVelocity()
    {
        if (!reachedEnd)
        {
            Vector2 direction = (currentPath.vectorPath[currentWaypoint] - transform.position).normalized;
            velocity = direction * movementSpeed;
        }
    }
}
