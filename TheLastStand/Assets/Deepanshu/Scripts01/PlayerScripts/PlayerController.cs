using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum CoverState
{
    NotInCover,
    InCover,
    InCoverColliding
}
public class PlayerController : MonoBehaviour, IDamageable
{ 
    private Inputs _controls;
    private Vector2 _movement;
    private Vector2 _lookDelta;
    private Vector2 _coverMovement;

    [SerializeField] private int maxHealth = 100;
    internal int CurrentHealth;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] public Transform shootPoint;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float adsMoveSpeed = 3f;
    [SerializeField] private float coverAdsMoveSpeed = 2f;
    [SerializeField] private Rigidbody rb;

    [Header("Mouse Look Settings")]
    [SerializeField] private float playerPitchClamp = 80f;
    [SerializeField] private float playerYawClamp = 80f;
    [SerializeField] private float coverPitchClamp = 80f;
    [SerializeField] private float coverYawClamp = 80f;
    [SerializeField] private float yawSpeed = 2f;
    [SerializeField] private float pitchSpeed = 2f;

    private float _yaw;
    private float _pitch;
    private bool _isAds = false;
    private bool _isCoverAds = false;

    [Header("Camera & ADS Settings")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraDamping = 5f;
    [SerializeField] private Vector3 leftShoulderPos = new Vector3(-0.5f, 1.5f, -3f);
    [SerializeField] private Vector3 rightShoulderPos = new Vector3(0.5f, 1.5f, -3f);
    [SerializeField] private float defaultFOV = 90f;
    [SerializeField] private float adsFOV = 40f;
    [SerializeField] private float coverAdsFOV = 30f;
    private ShootiController _shootiController;

    [Header("Cover System")]
    [SerializeField] private LayerMask coverLayer;
    [SerializeField] private float coverCheckDistance = 2f;
    private Transform _leftCoverPoint; 
    private Transform _rightCoverPoint;
    [SerializeField] private float coverMoveSpeed = 2f;

    [Header("Cover Camera Offsets")]
    [SerializeField] private Vector3 leftShoulderOffset = new Vector3(-1f, 0f, 0f);
    [SerializeField] private Vector3 rightShoulderOffset = new Vector3(1f, 0f, 0f);

    private CapsuleCollider _capsule;
    [FormerlySerializedAs("_isInCover")] public bool isInCover = false;
    [FormerlySerializedAs("_currentCover")] public Transform currentCover;

    private Vector3 _coverDirection;
    private Transform _closestCoverPoint;
    private bool _isLeftShoulder = true;

    [SerializeField] private Animator animator;
    private float _turnDirection;
    private float _turnSmoothTime = 0.1f;
    private bool _isHit = false;
    private float _hitDuration = 0.5f;
    private float _hitTimer = 0f;
    [SerializeField] private MultiAimConstraint[] aimConstraints;
    [SerializeField] private MultiAimConstraint[] coverAimConstraints;
    private float _moveX, _moveY;
    private int _coverLayerIndex = 3;

    [SerializeField]
    private GameObject objectToFlip;

    [Header("Cover Camera Settings")]
    [SerializeField] private Camera coverCamera;
    [SerializeField] private float coverCameraDistance = 2f;
    [SerializeField] private Vector3 coverCameraOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private Transform playerTarget;

    [FormerlySerializedAs("_coverState")] public CoverState coverState = CoverState.NotInCover;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform target1;

    [SerializeField]
    private Vector3 lastCoverPosition;
    public GameSettings gameSettings;
    public CoverState CoverState { get; private set; }

    internal bool IsPaused = false;

    void Start()
    {
       
        UpdateAimConstraints();
        CurrentHealth = maxHealth;
        UpdateHealthUI();
        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = leftShoulderPos;
            playerCamera.fieldOfView = defaultFOV;
        }
        if (coverCamera != null)
        {
            coverCamera.gameObject.SetActive(false);
        }
        animator = GetComponentInChildren<Animator>();
        _shootiController.playerCamera = playerCamera;
        _shootiController.coverCamera = coverCamera;
    }
    private void Awake()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _controls = new Inputs();
        _shootiController = GetComponent<ShootiController>();
        _controls.PlayerMovement.Movement.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Movement.canceled += ctx => _movement = Vector2.zero;
        _controls.PlayerMovement.CoverMovement.performed += ctx => _coverMovement = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.CoverMovement.canceled += ctx => _coverMovement = Vector2.zero;
        _controls.PlayerMovement.Look.performed += ctx => _lookDelta = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Look.canceled += ctx => _lookDelta = Vector2.zero;
        _controls.PlayerMovement.ADS.performed += ctx => ToggleAds(true);
        _controls.PlayerMovement.ADS.canceled += ctx => ToggleAds(false);
        _controls.PlayerMovement.CoverADS.performed += ctx => ToggleCoverAds(true);
        _controls.PlayerMovement.CoverADS.canceled += ctx => ToggleCoverAds(false);
        _controls.PlayerMovement.ShoulderChange.performed += ctx => HandleShoulderSwitch();
        _controls.PlayerMovement.Cover.started += ctx => TakeCover();
        _yaw = transform.eulerAngles.y;
        _pitch = 0f;
    }
    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
    private void Update()
    {
        if (!IsPaused)
        {
            HandleMovement();
            HandleLook();
            UpdateCamera();
            if (isInCover)
            {
                HandleCoverMovement();
                UpdateCoverCamera();
             
                if (coverState == CoverState.InCoverColliding)
                {
                    bool isMoving = _movement.magnitude > 0.1f;
                    animator.SetBool("isInCoverColliding", !isMoving);
                    animator.SetBool("IsInCover", isMoving);
                }
            }
            if (_isHit)
            {
                _hitTimer += Time.deltaTime;
                if (_hitTimer >= _hitDuration)
                {
                    _isHit = false;
                    animator.SetBool("IsHit", false);
                    _hitTimer = 0f;
                }
            }
            UpdateObjectToFlipRotation();
            UpdateAimConstraints();
        }
    }
    private void HandleMovement()
    {
        if (isInCover || IsPaused) return;
        float currentMoveSpeed = _isAds ? adsMoveSpeed : (_isCoverAds ? coverAdsMoveSpeed : moveSpeed);
        Vector3 inputDir = new Vector3(_movement.x, 0, _movement.y);
        inputDir = transform.TransformDirection(inputDir);
        rb.linearVelocity = new Vector3(inputDir.x * currentMoveSpeed, rb.linearVelocity.y, inputDir.z * currentMoveSpeed);
        float targetMoveX = _isLeftShoulder ? -_movement.x : _movement.x;
        float targetMoveY = _movement.y;
        _moveX = Mathf.Lerp(_moveX, targetMoveX, Time.deltaTime * 10f);
        _moveY = Mathf.Lerp(_moveY, targetMoveY, Time.deltaTime * 10f);
        animator.SetFloat("MoveX", _moveX);
        animator.SetFloat("MoveY", _moveY);
    }
    public void SetAimSensitivity(float sensitivity)
    {
        gameSettings.aimSensitivity = sensitivity;
    }
    private void HandleLook()
    {
        float pitchClamp = isInCover ? coverPitchClamp : playerPitchClamp;
        float yawClamp = isInCover ? coverYawClamp : playerYawClamp;

        if (isInCover)
        {
            _yaw += _lookDelta.x * yawSpeed * gameSettings.aimSensitivity;
            _pitch -= _lookDelta.y * pitchSpeed * gameSettings.aimSensitivity;
            _pitch = Mathf.Clamp(_pitch, -pitchClamp, pitchClamp);
        }
        else
        {
            float previousYaw = _yaw;
            _yaw += _lookDelta.x * yawSpeed * gameSettings.aimSensitivity;
            _pitch -= _lookDelta.y * pitchSpeed * gameSettings.aimSensitivity;
            _pitch = Mathf.Clamp(_pitch, -pitchClamp, pitchClamp);
            transform.rotation = Quaternion.Euler(0, _yaw, 0);
            if (cameraPivot != null)
                cameraPivot.localRotation = Quaternion.Euler(_pitch, 0, 0);
            float targetTurnDirection = _yaw - previousYaw;
            bool isTurning = Mathf.Abs(targetTurnDirection) > 0.1f && _movement.magnitude < 0.1f;
            _turnDirection = Mathf.Lerp(_turnDirection, targetTurnDirection, Time.deltaTime / _turnSmoothTime);
            animator.SetBool("IsTurning", isTurning);
            animator.SetFloat("TurnDirection", _turnDirection);
        }
    }
    private void UpdateCamera()
    {
        if (playerCamera != null && !isInCover)
        {
            Vector3 targetPosition = _isLeftShoulder ? leftShoulderPos : rightShoulderPos;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPosition, Time.deltaTime * cameraDamping);
            float targetFOV = _isAds ? adsFOV : defaultFOV;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * cameraDamping);
        }
    }
    private void UpdateCoverCamera()
    {
        if (coverCamera != null && playerTarget != null && isInCover)
        {
            Vector3 targetPosition = transform.position + (transform.forward * -coverCameraDistance) + coverCameraOffset;
            coverCamera.transform.position = Vector3.Lerp(coverCamera.transform.position, targetPosition, Time.deltaTime * cameraDamping);
            Quaternion targetRotation = Quaternion.LookRotation(playerTarget.position - coverCamera.transform.position);
            targetRotation = Quaternion.Euler(_pitch, targetRotation.eulerAngles.y + _yaw, targetRotation.eulerAngles.z);
            coverCamera.transform.rotation = Quaternion.Lerp(coverCamera.transform.rotation, targetRotation, Time.deltaTime * cameraDamping);
            float targetFOV = _isCoverAds ? coverAdsFOV : defaultFOV;
            coverCamera.fieldOfView = Mathf.Lerp(coverCamera.fieldOfView, targetFOV, Time.deltaTime * cameraDamping);
        }
    }
    private void ToggleAds(bool isActive)
    {
        if (isInCover)
        {
            _isAds = false;
            return;
        }
        _isAds = isActive;
    }
    private void ToggleCoverAds(bool isActive)
    {
        if (!isInCover)
        {
            _isCoverAds = false;
            return;
        }
        _isCoverAds = isActive;
    }
    private void HandleShoulderSwitch()
    {
        if (isInCover)
        {
            CoverShoulderSwitch();
        }
        else
        {
            SwitchShoulder();
        }
    }
    private void SwitchShoulder()
    {
        _isLeftShoulder = !_isLeftShoulder;
        Vector3 targetPosition = _isLeftShoulder ? leftShoulderPos : rightShoulderPos;
        playerCamera.transform.localPosition = new Vector3(targetPosition.x, playerCamera.transform.localPosition.y, playerCamera.transform.localPosition.z);
    }
    private void CoverShoulderSwitch()
    {
        _isLeftShoulder = !_isLeftShoulder;
        coverCameraOffset = _isLeftShoulder ? leftShoulderOffset : rightShoulderOffset;
        UpdateCoverCamera();
        _coverDirection = (_rightCoverPoint.position - _leftCoverPoint.position).normalized;
        Vector3 perpendicularDirection = Vector3.Cross(_coverDirection, Vector3.up);
        transform.rotation = Quaternion.LookRotation(perpendicularDirection);
    }
    private void TakeCover()
    {
        if (isInCover)
        {
            ExitCover();
            return;
        }
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(origin, transform.forward, out hit, coverCheckDistance, coverLayer))
        {
            animator.SetBool("IsTakingCover", true);
            var cover = hit.collider.gameObject;
            _leftCoverPoint = cover.transform.Find("LeftCoverPoint");
            _rightCoverPoint = cover.transform.Find("RightCoverPoint");

            if (_leftCoverPoint != null && _rightCoverPoint != null)
            {
                EnterCover(hit.point);
            }
        }
        animator.SetBool("IsTakingCover", false);
    }
    private void EnterCover(Vector3 coverPosition)
    {
        isInCover = true;
        currentCover = null;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = coverPosition;
        _coverDirection = (_rightCoverPoint.position - _leftCoverPoint.position).normalized;
        Vector3 perpendicularDirection = Vector3.Cross(_coverDirection, Vector3.up);
        transform.rotation = Quaternion.LookRotation(perpendicularDirection);

        animator.SetBool("IsInCover", true);
        animator.SetFloat("CoverMovement", 0);
        animator.SetLayerWeight(_coverLayerIndex, 1f);

        lastCoverPosition = coverPosition;
        CoverState = CoverState.InCover;
        NotifyEnemiesOfCoverState();

        foreach (var constraint in aimConstraints)
        {
            constraint.weight = 0f;
        }
        UpdateObjectToFlipRotation();
        if (coverCamera != null)
        {
            coverCamera.gameObject.SetActive(true);
        }
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
        }
        transform.localScale = Vector3.one;
    }
    private void HandleCoverMovement()
    {
        float sideMove = _coverMovement.x;
        Vector3 moveDirection = _coverDirection;
        Vector3 newPosition = transform.position + (moveDirection * (sideMove * coverMoveSpeed * Time.deltaTime));
        float leftBound = Vector3.Dot(newPosition - _leftCoverPoint.position, moveDirection);
        float rightBound = Vector3.Dot(newPosition - _rightCoverPoint.position, moveDirection);
        if (leftBound >= 0 && rightBound <= 0)
        {
            rb.MovePosition(newPosition);
        }
        animator.SetFloat("CoverMovement", sideMove);
    }
    private void ExitCover()
    {
        isInCover = false;
        currentCover = null;
        animator.SetLayerWeight(_coverLayerIndex, 0f);
        animator.SetBool("IsInCover", false);
        foreach (var constraint in aimConstraints)
        {
            constraint.weight = 1f;
        }
        lastCoverPosition = Vector3.zero;
        CoverState = CoverState.NotInCover;
        UpdateObjectToFlipRotation();
        if (coverCamera != null)
        {
            coverCamera.gameObject.SetActive(false);
        }
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }
        transform.localScale = Vector3.one;
        NotifyEnemiesOfCoverState();
    }
    private void NotifyEnemiesOfCoverState()
    {
        EnemyBase[] enemies = Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            enemy.SetPlayerCoverState(CoverState, lastCoverPosition);
        }
    }
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        _isHit = true;
        animator.SetBool("IsHit", true);
        UpdateHealthUI();
        if (CurrentHealth == 0)
            Die();
    }
    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"Player Health: {CurrentHealth}";
    }
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        UpdateHealthUI();
    }
    private void Die()
    {
        GameOverScript.Instance.GameOver();
       
    }
    private void UpdateAimConstraints()
    {
        switch (coverState)
        {
            case CoverState.NotInCover:
                foreach (var constraint in aimConstraints)
                {
                    constraint.weight = 1f;
                }
                foreach (var constraint in coverAimConstraints)
                {
                    constraint.weight = 0f;
                }
                animator.SetBool("isInCoverColliding", false);
                break;

            case CoverState.InCover:
                foreach (var constraint in aimConstraints)
                {
                    constraint.weight = 0f;
                }
                foreach (var constraint in coverAimConstraints)
                {
                    constraint.weight = 0f;
                }
                animator.SetBool("isInCoverColliding", false);
                break;

            case CoverState.InCoverColliding:
                foreach (var constraint in aimConstraints)
                {
                    constraint.weight = 0f;
                }
                foreach (var constraint in coverAimConstraints)
                {
                    constraint.weight = 1f;
                }
                animator.SetBool("isInCoverColliding", true);
                break;
        }
    }
    private void UpdateObjectToFlipRotation()
    {
        if (isInCover && !Physics.CheckSphere(transform.position, 0.5f, LayerMask.GetMask("CoverCorner")))
        {
            coverState = CoverState.InCover;
            objectToFlip.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            coverState = isInCover ? CoverState.InCoverColliding : CoverState.NotInCover;
            objectToFlip.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}

