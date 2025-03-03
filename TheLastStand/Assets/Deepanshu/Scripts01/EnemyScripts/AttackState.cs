using UnityEngine;

public class AttackState : EnemyBaseState
{
    private float shootTimer;
    private float attackDuration = 3f; 
    public AttackState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) {}

    public override void EnterState()
    {
        shootTimer = enemy.fireRate;
    }
    public override void UpdateState()
    {
        if (!enemy.IsStationary()) return; 
        attackDuration -= Time.deltaTime;
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            shootTimer = enemy.fireRate;
            enemy.Shoot();
        }
        if (attackDuration <= 0) 
        {
            stateMachine.ChangeState(new PatrolState(stateMachine, enemy));
        }
    }
    public override void ExitState() {}
}
