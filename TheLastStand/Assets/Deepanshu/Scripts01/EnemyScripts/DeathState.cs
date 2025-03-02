using UnityEngine;

public class DeathState : EnemyBaseState
{
    public DeathState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) {}
    public override void EnterState()
    {
        Debug.Log("Enemy is dead!");
        GameObject.Destroy(enemy.gameObject, 2f);
    }
    public override void UpdateState() { }
    public override void ExitState() { }
}
