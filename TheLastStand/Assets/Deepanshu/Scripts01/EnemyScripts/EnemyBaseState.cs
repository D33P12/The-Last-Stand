using UnityEngine;

public abstract class EnemyBaseState 
{
    protected EnemyStateMachine StateMachine;
    protected EnemyBase Enemy;
    public EnemyBaseState(EnemyStateMachine stateMachine, EnemyBase enemy)
    {
        this.StateMachine = stateMachine;
        this.Enemy = enemy;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public virtual void FixedUpdateState() { }
}
