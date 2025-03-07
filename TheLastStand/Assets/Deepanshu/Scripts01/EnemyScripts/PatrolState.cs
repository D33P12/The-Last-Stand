using UnityEngine;

public class PatrolState : EnemyBaseState
{
    private bool _reachedPoint = false;
    private Vector3 _randomDestination;
    public PatrolState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) {}
    public override void EnterState()
    {
        SetRandomDestination();
        _reachedPoint = false;
    }
    public override void UpdateState()
    {
        if (Enemy.IsStationary() && !_reachedPoint)
        {
            _reachedPoint = true;
            StateMachine.ChangeState(new AttackState(StateMachine, Enemy)); 
        }
    }
    public override void ExitState() {}
    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f; 
        randomDirection += Enemy.transform.position; 
        randomDirection.y = Enemy.transform.position.y; 
        
        Enemy.agent.SetDestination(randomDirection);
    }
}