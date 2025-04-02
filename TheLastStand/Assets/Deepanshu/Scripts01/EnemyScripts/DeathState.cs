using UnityEngine;

public class DeathState : EnemyBaseState
{
    public DeathState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) {}
    public override void EnterState()
    {
        Enemy.agent.updateRotation = false;
        if (Enemy.dropPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, Enemy.dropPrefabs.Length);
            Vector3 spawnPosition = Enemy.dropSpawnPoint != null ? Enemy.dropSpawnPoint.position : Enemy.transform.position;
            Object.Instantiate(Enemy.dropPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        }
        Enemy.Animator.SetTrigger("EnemyDeath");
        GameObject.Destroy(Enemy.gameObject, 15f);
    }
    public override void UpdateState() { }
    public override void ExitState() { }
}
