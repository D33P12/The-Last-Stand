using UnityEngine;
public class EnemyStateMachine
{
    public EnemyBaseState CurrentState { get; private set; }
    public void ChangeState(EnemyBaseState newState)
    {
        if (CurrentState == newState) return;
        CurrentState?.ExitState();
        CurrentState = newState;
        CurrentState?.EnterState();
    }
    public void Update()
    {
        CurrentState?.UpdateState();
    }
    public void FixedUpdate()
    {
        CurrentState?.FixedUpdateState();
    }
}
