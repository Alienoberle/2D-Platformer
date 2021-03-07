using UnityEngine;

public class Idle : IState
{
    public string name {get {return "Idle";}}
    private readonly EnemyPathfinding enemyMovement;
    private readonly Animator animator;

    public Idle(EnemyPathfinding enemyMovement, Animator animator)
    {
        this.enemyMovement = enemyMovement;
        this.animator = animator;
    }
    public void Tick()
    {
    }
    public void OnEnter()
    {
        animator.SetTrigger("Idle");
    }
    public void OnExit()
    {
        animator.ResetTrigger("Idle");
    }
}
