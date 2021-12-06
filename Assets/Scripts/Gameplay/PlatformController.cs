using System.Collections.Generic;
using UnityEngine;


/*
 * Basic set up of the script
 * https://youtu.be/z20wHJSXk98?t=901 
 */


public class PlatformController : RaycastController
{
    // needed to automatically sync the collider 2d with the transform https://docs.unity3d.com/ScriptReference/Physics2D-autoSyncTransforms.html
    //public static bool autoSyncTransforms; 

    public LayerMask passengerMask;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, PlayerController> passengerDictionary = new Dictionary<Transform, PlayerController>();

    [SerializeField] private Rigidbody2D rB2D;
    private float forceMultiplier = 50.0f;
    public float speed;
    [Range(1, 3)]
    [SerializeField] private float easeAmount = 1;
    [SerializeField] private float waitTime;
    [SerializeField] private bool isCyclic;

    private int fromWaypointIndex;
    private float percentBetweenWaypoints;
    private float nextMoveTime;

    public Vector3[] localWaypoints;
    private Vector3[] globalWaypoints;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();


        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateRaycastOrigins();
        Vector2 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);
        MovePassengers(true);
        rB2D.velocity = velocity * forceMultiplier;
        MovePassengers(false);
    }

    private float Ease(float x)
    {
        if (easeAmount == 0)
        {
            easeAmount += 1;
        }

        return Mathf.Pow(x, easeAmount) / (Mathf.Pow(x, easeAmount) + Mathf.Pow(1 - x, easeAmount));
    }

    Vector2 CalculatePlatformMovement()
    {
        // do not move platform if waittime is still "active"
        if (Time.time < nextMoveTime)
        {
            return Vector2.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length; // make waypoints start from Index 0 again 
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;

        float distanceBetweenWaypoints = Vector2.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.fixedDeltaTime * speed / distanceBetweenWaypoints;

        // ease the movement
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints); // clamp it to make sure we get no weird results from the ease fucnction
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPosition = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        // if we reached the goal waypoint
        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            nextMoveTime = Time.time + waitTime; // each time we reach a waypoint we set next move time

            if (!isCyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
        }

        return newPosition - transform.position;
    }


    void MovePassengers(bool beforeMovingPlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            if (!passengerDictionary.ContainsKey(passenger.passengerTransform))
            {
                passengerDictionary.Add(passenger.passengerTransform, passenger.passengerTransform.GetComponent<PlayerController>());
            }
            if (passenger.movingBeforePlatform == beforeMovingPlatform)
            {
                passengerDictionary[passenger.passengerTransform].platformVelocity = passenger.passengerVelocity * forceMultiplier;
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>(); // particulariy good at adding and checking if anything is part of the list very fast
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // Vertically moving platform, pushing the 'passengers' up or down
        if (velocity.y != 0)
        {
            float rayLenght = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLenght, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLenght, Color.blue);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionX == 1) ? velocity.x : 0; // we do not want to move the player along with the platform if he currently is below it
                        float pushY = velocity.y - ((hit.distance - skinWidth) * directionY); // subtract the possible gap and skinwidth between platform and passenger

                        // Add new passenger movement info struct to the passengerMovement list
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }

        // Horizontally moving platform, pushing the 'passengers' left or right
        if (velocity.x != 0)
        {
            float rayLenght = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {

                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLenght, Color.blue);

                if (hit)
                {

                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth; // 0 would be correct, but add small downwardforce allows the passenger check below itself, to fix jumping issue

                        // Add new passenger movement info struct to the passengerMovement list
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), false, true));
                    }
                }
            }
        }

        // Passenger on top of a horizontally or downward moving platform, 
        // moving the 'passengers' down, left or right overwriting vertical and horizontal movement 
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLenght = skinWidth * 3; // just a small ray detecting player on top

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLenght, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * rayLenght, Color.blue);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        // Add new passenger movement info struct to the passengerMovement list
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    struct PassengerMovement
    {
        public Transform passengerTransform;
        public Vector2 passengerVelocity;
        public bool standingOnPlatform;
        public bool movingBeforePlatform;

        public PassengerMovement(Transform _transform, Vector2 _passengervelocity, bool _standingOnPlatform, bool _movingBeforePlatform)
        {
            passengerTransform = _transform;
            passengerVelocity = _passengervelocity;
            standingOnPlatform = _standingOnPlatform;
            movingBeforePlatform = _movingBeforePlatform;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.yellow;
            float size = 0.1f;
            col2D = GetComponent<BoxCollider2D>();
            Bounds bounds = col2D.bounds;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                // when the game is playing we do not want the waypoints to move with the platform
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;

                // draw the size of the platform box collider
                Gizmos.DrawLine(globalWaypointPos + new Vector3(-bounds.size.x / 2, -bounds.size.y / 2), globalWaypointPos + new Vector3(-bounds.size.x / 2, +bounds.size.y / 2));
                Gizmos.DrawLine(globalWaypointPos + new Vector3(+bounds.size.x / 2, -bounds.size.y / 2), globalWaypointPos + new Vector3(+bounds.size.x / 2, +bounds.size.y / 2));
                Gizmos.DrawLine(globalWaypointPos + new Vector3(-bounds.size.x / 2, -bounds.size.y / 2), globalWaypointPos + new Vector3(+bounds.size.x / 2, -bounds.size.y / 2));
                Gizmos.DrawLine(globalWaypointPos + new Vector3(-bounds.size.x / 2, +bounds.size.y / 2), globalWaypointPos + new Vector3(+bounds.size.x / 2, +bounds.size.y / 2));

                // draw a small cross at global waypoint position
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
