using UnityEngine;

public class PatrolState : EnemyBaseState
{
    private float patrolTimer;
    public PatrolState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) {}
    public override void EnterState()
    {
        enemy.MoveToNextPatrolPoint();
    }
    public override void UpdateState()
    {
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0)
        {
            stateMachine.ChangeState(new AttackState(stateMachine, enemy));
        }
    }
    public override void ExitState() {}
}
