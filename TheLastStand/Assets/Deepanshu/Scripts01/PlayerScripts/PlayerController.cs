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
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float adsMoveSpeed = 3f;  
    [Header("Recoil (Shooting)")]
    [SerializeField] private float recoilAmount = 0.2f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    //[SerializeField] private float yawClamp = 90f;
    [SerializeField] private float pitchClamp = 80f;
    private float _baseYaw;
    private float _yaw;
    private float _pitch;
    private bool _isAds = false;
    [Header("Camera & ADS Settings")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraDamping = 5f;
    [SerializeField] private Vector3 defaultCamLocalPos = new Vector3(0.5f, 1.5f, -3f);
    [SerializeField] private Vector3 adsCamLocalPos = new Vector3(0.5f, 1.5f, -1.5f);
    [SerializeField] private float defaultFOV = 90f;
    [SerializeField] private float adsFOV = 40f;
    private ShootiController _shootiController;
    
    [Header("Shoulder Switching")]
    [SerializeField] private Vector3 leftShoulderPos = new Vector3(-0.5f, 1.5f, -3f);
    [SerializeField] private Vector3 rightShoulderPos = new Vector3(0.5f, 1.5f, -3f);
    
    private bool _isLeftShoulder = false;

    void Start()
    {
        _currentHealth = maxHealth;
        UpdateHealthUI();
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
        _baseYaw = transform.eulerAngles.y;
        _yaw = _baseYaw;
        _pitch = 0f;
        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = defaultCamLocalPos;
            playerCamera.fieldOfView = defaultFOV;
        }
    }
    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    private void Update()
    {
        HandleMovement();
        HandleLook();
        UpdateCamera();
        if (_isAds)
        {
            moveSpeed = 2f; 
        }
        else
        {
            moveSpeed = 5f; 
        }
    }
    private void HandleMovement()
    {
       
        float currentMoveSpeed = moveSpeed;

        if (_isAds) currentMoveSpeed = adsMoveSpeed; 
        Vector3 inputDir = new Vector3(_movement.x, 0, _movement.y);
        inputDir = transform.TransformDirection(inputDir);
        rb.linearVelocity = new Vector3(inputDir.x * moveSpeed, rb.linearVelocity.y, inputDir.z * moveSpeed);
    }
    private void HandleLook()
    {
        _yaw += _lookDelta.x * mouseSensitivity;
        _pitch -= _lookDelta.y * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, -pitchClamp, pitchClamp);
        //_yaw = Mathf.Clamp(_yaw, _baseYaw - yawClamp, _baseYaw + yawClamp);
        transform.rotation = Quaternion.Euler(0, _yaw, 0);
        if (cameraPivot != null) cameraPivot.localRotation = Quaternion.Euler(_pitch, 0, 0);
    }
    private void UpdateCamera()
    {
        if (playerCamera != null)
        {
            Vector3 targetPosition = _isAds ? adsCamLocalPos : (_isLeftShoulder ? leftShoulderPos : rightShoulderPos);
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
        {
            Die();
        }
    }
    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"Player Health: {_currentHealth}";
    }
    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);
        Debug.Log("Health Refilled: " + _currentHealth);
        UpdateHealthUI();
    }
    private void Die()
    {
        Debug.Log("Player Died!");
    }
}


