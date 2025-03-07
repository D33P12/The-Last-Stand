using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour, IInteractable
{
    [SerializeField] public NavMeshAgent agent;
    private Transform _player;
    public Transform firePoint;
    [Header("Enemy Settings")]
    public float enemyRange = 10f;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;
    public int bulletsPerRound = 3;
    public float randomMoveRadius = 10f;
    private bool _isBeingTargeted = false;
    private EnemyStateMachine _stateMachine;
    [SerializeField] private int maxHealth = 100;
    private int _currentHealth;
    [SerializeField] private GameObject[] dropPrefabs;
    [SerializeField] private Transform dropSpawnPoint;
    private bool _isDead = false;
    [Header("Health UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Canvas healthCanvas;
    private Camera _playerCamera;
    private ShootiController _shootiController;
    public void SetPlayer(Transform playerTransform)
    {
        _player = playerTransform;
    }
    public void SetCamera(Camera cam)
    {
        _playerCamera = cam;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _stateMachine = new EnemyStateMachine();
        _stateMachine.ChangeState(new PatrolState(_stateMachine, this));
        _currentHealth = maxHealth;
        InitializeHealthBar();
        UpdateHealthUI();
        if (WaveManager.Instance != null)
        {
            _playerCamera = WaveManager.Instance.GetPlayerCamera();
        }
        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(false);
    }
    void Update()
    {
        _stateMachine.Update();
        if (_player != null && DetectPlayer())
        {
            RotateTowardsPlayer();
        }
        CheckPlayerAim();
        RotateHealthBar(); 
    }
    public void RotateTowardsPlayer()
    {
        if (_player == null) return;
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    private void RotateHealthBar()
    {
        if (_playerCamera == null || healthBar == null) return;
        healthBar.transform.LookAt(healthBar.transform.position + _playerCamera.transform.forward);
    }
    public bool DetectPlayer()
    {
        if (_player == null) return false;
        return Vector3.Distance(transform.position, _player.position) <= enemyRange;
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
    {if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(true);
        if (_isDead) return;
        _currentHealth -= damage;
        if (_currentHealth < 0) _currentHealth = 0;
        UpdateHealthUI();
        if (_currentHealth == 0)
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
            healthBar.value = _currentHealth;
    }
    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        if (dropPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, dropPrefabs.Length);
            Vector3 spawnPosition = dropSpawnPoint != null ? dropSpawnPoint.position : transform.position;
            Instantiate(dropPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        }
        WaveManager.Instance.OnEnemyDeath();
        Destroy(gameObject, 2f);
    }
    private void CheckPlayerAim()
    {
        if (_shootiController == null || _shootiController.shootCamera == null) return;
        Ray ray = _shootiController.shootCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject == gameObject)
        {
            _isBeingTargeted = true;
        }
        else
        {
            _isBeingTargeted = false;
        }
        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(_isBeingTargeted);
    }
}
