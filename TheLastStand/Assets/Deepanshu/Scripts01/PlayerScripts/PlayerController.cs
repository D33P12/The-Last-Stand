using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CameraState
{
    Default,
    Ads
}
public class PlayerController : MonoBehaviour, IDamageable
{ 
    private Inputs _controls;
    private Vector2 _movement;
    private Vector2 _lookDelta;

    [SerializeField] private int maxHealth = 100;
    private int _currentHealth;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] public Transform shootPoint;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float adsMoveSpeed = 3f;
    [SerializeField] private Rigidbody rb;

    [Header("Recoil (Shooting)")]
    [SerializeField] private float recoilAmount = 0.2f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float pitchClamp = 80f;
    
    private float _yaw;
    private float _pitch;
    private bool _isAds = false;

    [Header("Camera & ADS Settings")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraDamping = 5f;
    [SerializeField] private Vector3 leftShoulderPos = new Vector3(-0.5f, 1.5f, -3f);
    [SerializeField] private Vector3 rightShoulderPos = new Vector3(0.5f, 1.5f, -3f);
    [SerializeField] private float defaultFOV = 90f;
    [SerializeField] private float adsFOV = 40f;
    private ShootiController _shootiController;
    
    [Header("Cover System")]
    [SerializeField] private LayerMask coverLayer;
    [SerializeField] private float coverCheckDistance = 2f;
    [SerializeField] private Transform leftCoverPoint;
    [SerializeField] private Transform rightCoverPoint;
    [SerializeField] private float coverMoveSpeed = 2f;
    
    private CapsuleCollider _capsule;
    public bool _isInCover = false;
    public Transform _currentCover;
    
    private Vector3 _coverDirection;
    private Transform _closestCoverPoint;
    private bool _isLeftShoulder = true;

    [SerializeField] private Animator animator;

    void Start()
    {
        _currentHealth = maxHealth;
        UpdateHealthUI();
        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = leftShoulderPos;
            playerCamera.fieldOfView = defaultFOV;
        }
    }
    private void Awake()
    {
        _controls = new Inputs();
        _shootiController = GetComponent<ShootiController>();
        _controls.PlayerMovement.Movement.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Movement.canceled += ctx => _movement = Vector2.zero;
        _controls.PlayerMovement.Look.performed += ctx => _lookDelta = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Look.canceled += ctx => _lookDelta = Vector2.zero;
        _controls.PlayerMovement.ADS.performed += ctx => ToggleAds(true);
        _controls.PlayerMovement.ADS.canceled += ctx => ToggleAds(false);
        _controls.PlayerMovement.ShoulderChange.performed += ctx => SwitchShoulder();
        _controls.PlayerMovement.Cover.started += ctx => TakeCover();
        _yaw = transform.eulerAngles.y;
        _pitch = 0f;
    }
    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
    private void Update()
    {
        HandleMovement();
        HandleLook();
        UpdateCamera();
        if (_isInCover)
        {
            HandleCoverMovement();
        }
    }
    private void HandleMovement()
    {
        if (_isInCover) return;
        
        float currentMoveSpeed = _isAds ? adsMoveSpeed : moveSpeed;
        Vector3 inputDir = new Vector3(_movement.x, 0, _movement.y);
        inputDir = transform.TransformDirection(inputDir);
        
        rb.linearVelocity = new Vector3(inputDir.x * currentMoveSpeed, rb.linearVelocity.y, inputDir.z * currentMoveSpeed);
        float moveX = _isLeftShoulder ? -_movement.x : _movement.x;
        float moveY = _movement.y;
        
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);
    }
    private void HandleLook()
    {
        _yaw += _lookDelta.x * mouseSensitivity;
        _pitch -= _lookDelta.y * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, -pitchClamp, pitchClamp);
        transform.rotation = Quaternion.Euler(0, _yaw, 0);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(_pitch, 0, 0);
    }
    private void UpdateCamera()
    {
        if (playerCamera != null)
        {
            Vector3 targetPosition = _isLeftShoulder ? leftShoulderPos : rightShoulderPos;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPosition, Time.deltaTime * cameraDamping);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, _isAds ? adsFOV : defaultFOV, Time.deltaTime * cameraDamping);
        }
    }
    private void ToggleAds(bool isActive)
    {
        _isAds = isActive;
    }
    private void SwitchShoulder()
    {
        _isLeftShoulder = !_isLeftShoulder;

        transform.localScale = new Vector3(_isLeftShoulder ? 1 : -1, 1, 1);
    }
    private void TakeCover()
    {
        if (_isInCover)
        {
            ExitCover();
            return;
        }
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(origin, transform.forward, out hit, coverCheckDistance, coverLayer))
        {
            Debug.Log("Cover found!");
            EnterCover(hit.transform);
        }
        else
        {
            Debug.Log("No cover found.");
        }
    }
    private void EnterCover(Transform cover)
    {
        _isInCover = true;
        _currentCover = cover;
        Vector3 coverPosition = new Vector3(transform.position.x, transform.position.y, cover.position.z); 
        transform.position = coverPosition;
        _coverDirection = (rightCoverPoint.position - leftCoverPoint.position).normalized;
    }
    private void HandleCoverMovement()
    {
        float sideMove = Input.GetAxis("Horizontal"); 
        Vector3 moveDirection = _coverDirection; 

        Vector3 newPosition = transform.position + (moveDirection * (sideMove * coverMoveSpeed * Time.deltaTime));
        float leftBound = Vector3.Dot(newPosition - leftCoverPoint.position, moveDirection);
        float rightBound = Vector3.Dot(newPosition - rightCoverPoint.position, moveDirection);

        if (leftBound >= 0 && rightBound <= 0)
        {
            rb.MovePosition(newPosition);
        }
    }
    private void ExitCover()
    {
        _isInCover = false;
        _currentCover = null;
    }
    public void ApplyRecoil(Vector3 shootDirection)
    {
        Vector3 recoilForce = shootDirection * recoilAmount;
        rb.AddForce(-recoilForce, ForceMode.Impulse);
    }
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        UpdateHealthUI();

        if (_currentHealth <= 0)
            Die();
    }
    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"Player Health: {_currentHealth}";
    }
    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }
    private void Die()
    {
        Debug.Log("Player Died!");
    }
}


