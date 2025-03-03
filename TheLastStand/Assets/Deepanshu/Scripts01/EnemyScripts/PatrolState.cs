using UnityEngine;

public class PatrolState : EnemyBaseState
{
    private bool reachedPoint = false;
    public PatrolState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) {}

    public override void EnterState()
    {
        enemy.MoveToNextPatrolPoint();
        reachedPoint = false;
    }
    public override void UpdateState()
    {
        if (enemy.IsStationary() && !reachedPoint)
        {
            reachedPoint = true;
            stateMachine.ChangeState(new AttackState(stateMachine, enemy)); 
        }
    }
    public override void ExitState() {}
}