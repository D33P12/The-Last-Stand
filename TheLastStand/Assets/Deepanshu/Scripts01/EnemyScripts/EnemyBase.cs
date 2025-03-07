using System.Collections;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour, IInteractable
{
    [SerializeField] public NavMeshAgent agent;
    private Transform player;
    public Transform firePoint;

    [Header("Enemy Settings")]
    public float enemyRange = 10f;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;
    public int bulletsPerRound = 3;
    public float randomMoveRadius = 10f;

    private EnemyStateMachine stateMachine;

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private GameObject[] dropPrefabs;
    [SerializeField] private Transform dropSpawnPoint;
    private bool isDead = false;

    [Header("Health UI")]
    [SerializeField] private Slider healthBar;

    private Camera playerCamera;

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }
    public void SetCamera(Camera cam)
    {
        playerCamera = cam;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new EnemyStateMachine();
        stateMachine.ChangeState(new PatrolState(stateMachine, this));
        currentHealth = maxHealth;

        InitializeHealthBar();
        UpdateHealthUI();

        if (WaveManager.Instance != null)
        {
            playerCamera = WaveManager.Instance.GetPlayerCamera();
        }
    }
    void Update()
    {
        stateMachine.Update();
        if (player != null && DetectPlayer())
        {
            RotateTowardsPlayer();
        }

        RotateHealthBar(); 
    }
    public void RotateTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    private void RotateHealthBar()
    {
        if (playerCamera == null || healthBar == null) return;

        healthBar.transform.LookAt(healthBar.transform.position + playerCamera.transform.forward);
    }
    public bool DetectPlayer()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= enemyRange;
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
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthUI();
        if (currentHealth == 0)
        {
            Die();
        }
    }
    private void InitializeHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }
    private void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;
    }
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        if (dropPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, dropPrefabs.Length);
            Vector3 spawnPosition = dropSpawnPoint != null ? dropSpawnPoint.position : transform.position;
            Instantiate(dropPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        }
        WaveManager.Instance.OnEnemyDeath();
        Destroy(gameObject, 2f);
    }
}
