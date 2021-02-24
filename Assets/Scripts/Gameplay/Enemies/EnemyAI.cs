using System;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(EnemyPathfinding))]
public class EnemyAI : MonoBehaviour
{
    private Vector3 startingPosition;
    private StateMachine stateMachine;
    private EnemyPathfinding enemyMovement;
    private GameObject player;
    private PlayerTrigger playerTrigger;
    
    [SerializeField] private Vector3[] waypoints;

    private void Awake()
    {
        startingPosition = transform.root.position;
        stateMachine = new StateMachine();
        enemyMovement = GetComponent<EnemyPathfinding>();
        player = Player.instance.transform.root.gameObject;
        playerTrigger = GetComponentInChildren<PlayerTrigger>();
 
        // Defines the possible States of the Enemy
        var idle = new Idle(this, enemyMovement);
        var patrol = new Patrol(enemyMovement, startingPosition, waypoints);
        var chase = new Chase(enemyMovement, player);

        //Add all possible transitions between states
        AddTransition(idle, patrol, HasPatrolRoute());
        AddTransition(patrol, chase, PlayerIsInRange());
        AddTransition(chase, patrol, LostPlayer());

        stateMachine.SetState(idle);

        stateMachine.AddAnyTransition(idle, HasNoPatrolRoute());

        void AddTransition(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        Func<bool> HasNoPatrolRoute() => () => waypoints == null;
        Func<bool> HasPatrolRoute() => () => waypoints != null;
        Func<bool> PlayerIsInRange() => () => playerTrigger.isPlayerInTrigger == true;
        Func<bool> LostPlayer() => () => playerTrigger.isPlayerInTrigger == false;
    }
    private void Update() => stateMachine.Tick();

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            string _stateText = stateMachine.currentState.name;

            GUIStyle customStyle = new GUIStyle();
            customStyle.fontSize = 20;   // can also use e.g. <size=30> in Rich Text
            customStyle.richText = true;
            Vector3 textPosition = transform.position + (Vector3.up * 0.5f);
            string richText = "<color=red><B>[" + _stateText + "]</B></color>";

            Handles.Label(textPosition, richText, customStyle);
        }

        if (waypoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.1f;
            BoxCollider2D collider = GetComponentInParent<BoxCollider2D>();
            Bounds bounds = collider.bounds;

            for (int i = 0; i < waypoints.Length; i++)
            {
                // when the game is playing we do not want the waypoints to move with the enemy
                Vector3 globalWaypointPos = (Application.isPlaying) ? waypoints[i] + startingPosition : waypoints[i] + transform.position;

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

