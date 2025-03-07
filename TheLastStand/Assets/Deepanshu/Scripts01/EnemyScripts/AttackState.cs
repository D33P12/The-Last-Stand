using UnityEngine;

public class AttackState : EnemyBaseState
{
    private float _shootTimer;
    private float _attackDuration = 3f; 
    public AttackState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) {}

    public override void EnterState()
    {
        _shootTimer = Enemy.fireRate;
    }
    public override void UpdateState()
    {
        if (!Enemy.IsStationary()) return; 
        _attackDuration -= Time.deltaTime;
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0)
        {
            _shootTimer = Enemy.fireRate;
            Enemy.Shoot();
        }
        if (_attackDuration <= 0) 
        {
            StateMachine.ChangeState(new PatrolState(StateMachine, Enemy));
        }
    }
    public override void ExitState() {}
}
