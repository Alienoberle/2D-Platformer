using System;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(EnemyCollision))]
public class EnemyPathfinding : MonoBehaviour
{
    private EnemyCollision enemyCollision;

    private Transform chaseTarget;
    private Vector3 target;
    private Vector3 velocity;
    public bool active { get; private set; }

    [Header("Movement")]
    public float movementSpeed;
    public float maximumSlopeAngle;
    public float gravity;
    [Header("Pathfinding")]
    public float nextWaypointDistance = 0.5f;

    private Seeker seeker;
    private Path currentPath;
    private int currentWaypoint = 0;
    private bool reachedEnd = false;

    public event Action OnTargetReached = delegate { };

    private void Awake()
    {
        enemyCollision = GetComponent<EnemyCollision>();
        enemyCollision.maxSlopeAngle = maximumSlopeAngle;
        seeker = GetComponent<Seeker>();
    }
    public void GoToTarget(Vector3 target)
    {
        this.target = target;
        UpdatePath();
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
            target = chaseTarget.position;
        }
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, target, OnPathCalculationComplete);
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
            enemyCollision.Move(velocity * Time.deltaTime, false);
        }      
    }

    private void CalculateVelocity()
    {
        if (!reachedEnd)
        {
            Vector2 direction = (currentPath.vectorPath[currentWaypoint] - transform.position).normalized;
            velocity.x = direction.x * movementSpeed;
            velocity.y = direction.y * movementSpeed;
        }
    }
}
