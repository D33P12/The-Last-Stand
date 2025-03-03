using UnityEngine;

public abstract class EnemyBaseState 
{
    protected EnemyStateMachine stateMachine;
    protected EnemyBase enemy;
    public EnemyBaseState(EnemyStateMachine stateMachine, EnemyBase enemy)
    {
        this.stateMachine = stateMachine;
        this.enemy = enemy;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public virtual void FixedUpdateState() { }
}
