using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] 
    private NavMeshAgent agent;
    public Transform[] patrolPoints;
    public Transform player;
    public LayerMask playerLayer;
    public Transform firePoint;
    [Header("Enemy Settings")] 
    public float enemyRange = 10f;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;
    public int bulletsPerRound = 3;
    private int currentPatrolIndex = 0;
    private EnemyStateMachine stateMachine;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new EnemyStateMachine();
        stateMachine.ChangeState(new PatrolState(stateMachine, this));
    }
    void Update()
    {
        stateMachine.Update();
        if (DetectPlayer())
        {
            RotateTowardsPlayer();
        }
    }
    public void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); 
    }
    public void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }
    public bool DetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= enemyRange;
    }
    public bool IsStationary()
    {
        return !agent.pathPending && agent.velocity.magnitude < 0.1f;
    }
    public void Shoot()
    {
        StartCoroutine(ShootingCoroutine());
    }
    IEnumerator ShootingCoroutine()
    {
        for (int i = 0; i < bulletsPerRound; i++)
        {
            GameObject bullet = BulletPool.Instance.GetBullet();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            if (bulletScript != null)
            {
                bulletScript.SetSpeed(bulletSpeed);
            }
            yield return new WaitForSeconds(fireRate);
        }
    }
}
