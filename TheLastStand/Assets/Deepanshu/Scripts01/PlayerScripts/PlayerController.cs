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
public class PlayerController : MonoBehaviour
{ 
    private Inputs _controls;
    private Vector2 _movement;
    private Vector2 _lookDelta;
    
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [SerializeField] public Transform shootPoint;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody rb;

    [Header("Recoil (Shooting)")]
    [SerializeField] private float recoilAmount = 0.2f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float yawClamp = 90f;
    [SerializeField] private float pitchClamp = 80f;
    private float baseYaw;
    private float yaw;
    private float pitch;
    private bool isADS = false;
    
    [Header("Camera & ADS Settings")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraDamping = 5f;
    [SerializeField] private Vector3 defaultCamLocalPos = new Vector3(0.5f, 1.5f, -3f);
    [SerializeField] private Vector3 adsCamLocalPos = new Vector3(0.5f, 1.5f, -1.5f);
    [SerializeField] private float defaultFOV = 90f;
    [SerializeField] private float adsFOV = 40f;
    private ShootiController shootiController;
    
    [Header("Shoulder Switching")]
    [SerializeField] private Vector3 leftShoulderPos = new Vector3(-0.5f, 1.5f, -3f);
    [SerializeField] private Vector3 rightShoulderPos = new Vector3(0.5f, 1.5f, -3f);
    
    private bool isLeftShoulder = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
    private void Awake()
    {
        _controls = new Inputs();
        shootiController = GetComponent<ShootiController>();
        _controls.PlayerMovement.Movement.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Movement.canceled += ctx => _movement = Vector2.zero;
        _controls.PlayerMovement.Look.performed += ctx => _lookDelta = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Look.canceled += ctx => _lookDelta = Vector2.zero;
        _controls.PlayerMovement.ADS.performed += ctx => ToggleADS(true);
        _controls.PlayerMovement.ADS.canceled += ctx => ToggleADS(false);
        _controls.PlayerMovement.ShoulderChange.performed += ctx => SwitchShoulder(); 
        baseYaw = transform.eulerAngles.y;
        yaw = baseYaw;
        pitch = 0f;
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
    }
    private void HandleMovement()
    {
        Vector3 inputDir = new Vector3(_movement.x, 0, _movement.y);
        inputDir = transform.TransformDirection(inputDir);
        rb.linearVelocity = new Vector3(inputDir.x * moveSpeed, rb.linearVelocity.y, inputDir.z * moveSpeed);
    }
    private void HandleLook()
    {
        yaw += _lookDelta.x * mouseSensitivity;
        pitch -= _lookDelta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);
        yaw = Mathf.Clamp(yaw, baseYaw - yawClamp, baseYaw + yawClamp);
        transform.rotation = Quaternion.Euler(0, yaw, 0);
        if (cameraPivot != null) cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
    private void UpdateCamera()
    {
        if (playerCamera != null)
        {
            Vector3 targetPosition = isADS ? adsCamLocalPos : (isLeftShoulder ? leftShoulderPos : rightShoulderPos);
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPosition, Time.deltaTime * cameraDamping);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, isADS ? adsFOV : defaultFOV, Time.deltaTime * cameraDamping);
        }
    }
    private void ToggleADS(bool isActive)
    {
        isADS = isActive;
    }
    private void SwitchShoulder()
    {
        isLeftShoulder = !isLeftShoulder;
    }
    public void ApplyRecoil(Vector3 shootDirection)
    {
        Vector3 recoilForce = shootDirection * recoilAmount;
        rb.AddForce(-recoilForce, ForceMode.Impulse);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"Player Health: {currentHealth}";
    }
    private void Die()
    {
        Debug.Log("Player Died!");
    }
}


