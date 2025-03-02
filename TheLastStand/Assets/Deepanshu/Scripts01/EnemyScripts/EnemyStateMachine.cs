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

        Debug.Log($"Enemy changed state to: {CurrentState.GetType().Name}");
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
