using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour, IInteractable
{
    public event Action<EnemyBase> OnDeath;

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

    [SerializeField] internal GameObject[] dropPrefabs;
    [SerializeField] internal Transform dropSpawnPoint;
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

    private float _grenadeTimer = 0f;
    private Vector3 _playerLastCoverPosition;
    private CoverState _playerCoverState;

    internal Animator Animator;
    private bool _isMoving = false;
    private Vector3 _moveDirection = Vector3.zero;
    private bool _isThrowingGrenade = false;
    [SerializeField]
    private float _shootingCooldown = 2f;
    private float _shootingCooldownTimer = 0f;

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
        Animator = GetComponentInChildren<Animator>();
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

        bool shouldMove = agent.velocity.magnitude > 0.1f;
        Vector3 moveDirection = agent.velocity.normalized;

        Animator.SetBool("isMoving", shouldMove);
        Animator.SetFloat("moveX", moveDirection.x);
        Animator.SetFloat("moveY", moveDirection.z);
        if (_player != null && DetectPlayer())
        {
            RotateTowardsPlayer();
        }
        CheckPlayerAim();
        RotateHealthBar();
        if (_playerCoverState == CoverState.InCover || _playerCoverState == CoverState.InCoverColliding)
        {
            _grenadeTimer += Time.deltaTime;
            if (_grenadeTimer >= grenadeThrowInterval)
            {
                ThrowGrenade();
                _grenadeTimer = 0f;
            }
        }
        else
        {
            _grenadeTimer = 0f;
        }
        Animator.SetBool("isThrowingGrenade", _isThrowingGrenade);

        // Update the shooting cooldown timer
        if (_shootingCooldownTimer > 0)
        {
            _shootingCooldownTimer -= Time.deltaTime;
        }
    }
    public void SetPlayerCoverState(CoverState state, Vector3 position)
    {
        _playerCoverState = state;
        _playerLastCoverPosition = position;
    }
    private void ThrowGrenade()
    {
        if (grenadePrefab != null && grenadeShootPoint != null)
        {
            _isThrowingGrenade = true;
            GameObject grenade = Instantiate(grenadePrefab, grenadeShootPoint.position, Quaternion.identity);
            Grenade grenadeScript = grenade.GetComponent<Grenade>();
            if (grenadeScript != null)
            {
                Vector3 displacement = _playerLastCoverPosition - grenadeShootPoint.position;
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
            Invoke("ResetThrowGrenade", 1f);
        }
    }
    private void ResetThrowGrenade()
    {
        _isThrowingGrenade = false;
    }
    public void MoveTo(Vector3 targetPosition)
    {
        _isMoving = true;
        _moveDirection = (targetPosition - transform.position).normalized;
        agent.SetDestination(targetPosition);
    }
    public void StopMoving()
    {
        _isMoving = false;
        _moveDirection = Vector3.zero;
        agent.ResetPath();
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

            yield return new WaitForSeconds(fireRate / bulletsPerRound);
        }
        _isShooting = false;

        // Start the cooldown timer
        _shootingCooldownTimer = _shootingCooldown;
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
        _isMoving = false;
        _isThrowingGrenade = false;
        agent.ResetPath();
        agent.isStopped = true;
        _stateMachine.ChangeState(new DeathState(_stateMachine, this));
        OnDeath?.Invoke(this);
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
