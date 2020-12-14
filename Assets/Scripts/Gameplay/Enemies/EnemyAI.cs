using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyAI : MonoBehaviour
{
    private StateMachine stateMachine;
    private EnemyMovement enemyMovement;

    private void Awake()
    {
        stateMachine = new StateMachine();
        enemyMovement = GetComponent<EnemyMovement>();

        var idle = new Idle(this, enemyMovement);
        var patrol = new Patrol(this, enemyMovement);
    }
}
