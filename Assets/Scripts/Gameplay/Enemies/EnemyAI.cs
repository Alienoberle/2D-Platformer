using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyAI : MonoBehaviour
{
    private StateMachine stateMachine;
    private EnemyMovement enemyMovement;
    
    private bool idle = false;
    [SerializeField] private bool chase = false;
    [SerializeField] private Vector3[] waypoints;
    [SerializeField] private Player player;


    private void Awake()
    {
        stateMachine = new StateMachine();
        enemyMovement = GetComponent<EnemyMovement>();
        player = Player.instance;

        var idle = new Idle(this, enemyMovement);
        var patrol = new Patrol(this, enemyMovement, waypoints);
        var chase = new Chase(this, enemyMovement, player.transform);

        void AddTransition(IState from, IState to, Func<bool> condition)
        {
            stateMachine.AddTransition(from, to, condition);
        }
        AddTransition(idle, patrol, HasPatrolRoute());
        AddTransition(patrol, chase, ChasePlayer());

        stateMachine.SetState(idle);

        stateMachine.AddAnyTransition(idle, () => this.idle == true);

        Func<bool> HasPatrolRoute() => () => waypoints != null;
        Func<bool> ChasePlayer() => () => this.chase == true;
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
                Vector3 globalWaypointPos = (Application.isPlaying) ? waypoints[i] : waypoints[i] + transform.position;

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

