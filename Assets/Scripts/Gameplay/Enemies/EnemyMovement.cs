using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    EnemyController enemyController;

    private Vector3 velocity;
    private float gravity;

    [Header("Movement")]
    public float movementSpeed;
    public float maximumSlopeAngle;
    [Header("Pathfinding")]
    public float nextWaypointDistance;

    private Seeker seeker;
    private Path currentPath;
    private int currentWaypoint = 0;
    private bool reachedEnd = false;

    void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        enemyController.maxSlopeAngle = maximumSlopeAngle;
        seeker = GetComponent<Seeker>();
        gravity = -1;

        InvokeRepeating("UpdatePath", 0f, 1f);
    }

    void Update()
    {
        CheckReachedEnd();
        CalculateVelocity();
        CheckDistanceToNextWaypoint();

        // Hand over the input and calcualted velocity to the enemycontroller handling the actual movement and collision
        enemyController.Move(velocity * Time.deltaTime, false);
    }

    private void CalculateVelocity()
    {
        Vector2 direction = (Vector2)(currentPath.vectorPath[currentWaypoint] - transform.position).normalized;
        velocity = direction * movementSpeed;
        Debug.Log(currentPath.vectorPath[currentWaypoint]);
        Debug.Log(direction);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
    }
    private void CheckDistanceToNextWaypoint()
    {
        float distance = Vector2.Distance(transform.position, currentPath.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
    private void CheckReachedEnd()
    {
        if (currentPath == null)
        {
            return;
        }
        if (currentWaypoint >= currentPath.vectorPath.Count)
        {
            reachedEnd = true;
            return;
        }
        else
        {
            reachedEnd = false;
        }
    }
    private void OnPathComplete(Path path)
    {
        if(!path.error)
        {
            currentPath = path;
            currentWaypoint = 0;
        }
    }
}
