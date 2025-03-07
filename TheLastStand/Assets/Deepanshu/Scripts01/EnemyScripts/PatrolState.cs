using UnityEngine;

public class PatrolState : EnemyBaseState
{
    private bool reachedPoint = false;
    private Vector3 randomDestination;
    public PatrolState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) {}
    public override void EnterState()
    {
        SetRandomDestination();
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
    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f; 
        randomDirection += enemy.transform.position; 
        randomDirection.y = enemy.transform.position.y; 
        
        enemy.agent.SetDestination(randomDirection);
    }
}