using UnityEngine;

public class AttackState : EnemyBaseState
{
    private float shootTimer;

    public AttackState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) {}
    public override void EnterState()
    {
        Debug.Log("Enemy entered ATTACK state.");
        shootTimer = enemy.fireRate;
    }
    public override void UpdateState()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0 && enemy.IsStationary())
        {
            shootTimer = enemy.fireRate;
            enemy.Shoot();
        }
        if (!enemy.DetectPlayer())
        {
            stateMachine.ChangeState(new PatrolState(stateMachine, enemy));
        }
    }
    public override void ExitState()
    {
        Debug.Log("Enemy exiting ATTACK state.");
    }
}
