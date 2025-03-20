using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour, IInteractable
{
     [SerializeField] public NavMeshAgent agent;
    private Transform _player;
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Enemy Settings")]
    public float enemyRange = 10f;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;
    public int bulletsPerRound = 3;
    public float randomMoveRadius = 10f;

    private bool _isBeingTargeted = false;
    private EnemyStateMachine _stateMachine;
    private bool _isShooting = false;

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
    
    [SerializeField]
    private float grenadeThrowInterval = 5f; 
    [SerializeField]
    private GameObject grenadePrefab;
    [SerializeField]
    private Transform grenadeShootPoint;

    private float grenadeTimer = 0f;
    private Vector3 playerLastCoverPosition;
    private CoverState playerCoverState;

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
     
        if (playerCoverState == CoverState.InCover || playerCoverState == CoverState.InCoverColliding)
        {
            grenadeTimer += Time.deltaTime;
            if (grenadeTimer >= grenadeThrowInterval)
            {
                ThrowGrenade();
                grenadeTimer = 0f;
            }
        }
        else
        {
            grenadeTimer = 0f; 
        }
    }
    public void SetPlayerCoverState(CoverState state, Vector3 position)
    {
        playerCoverState = state;
        playerLastCoverPosition = position;
    }
    private void ThrowGrenade()
    {
        if (grenadePrefab != null && grenadeShootPoint != null)
        {
            GameObject grenade = Instantiate(grenadePrefab, grenadeShootPoint.position, Quaternion.identity);
            Grenade grenadeScript = grenade.GetComponent<Grenade>();
            if (grenadeScript != null)
            {
                Vector3 displacement = playerLastCoverPosition - grenadeShootPoint.position;
                float distance = displacement.magnitude;
                float gravity = Physics.gravity.magnitude;
                float timeOfFlight = Mathf.Sqrt((2 * distance) / gravity);
                Vector3 velocity = new Vector3(
                    displacement.x / timeOfFlight,
                    gravity * timeOfFlight / 2,
                    displacement.z / timeOfFlight
                );

                grenadeScript.Launch(velocity);
            }
        }
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
        if (_isShooting || firePoint == null || bulletPrefab == null) return;
        _isShooting = true;
        StartCoroutine(ShootingCoroutine());
    }
    IEnumerator ShootingCoroutine()
    {
        for (int i = 0; i < bulletsPerRound; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(_player.position - firePoint.position));
            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            if (bulletScript != null)
            {
                bulletScript.SetSpeed(bulletSpeed);
            }

            Debug.Log($"Shooting bullet from {firePoint.position} towards {_player.position}");

            yield return new WaitForSeconds(fireRate / bulletsPerRound);
        }
        _isShooting = false;
    }
    public void TakeDamage(int damage)
    {
        if (healthCanvas != null)
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
