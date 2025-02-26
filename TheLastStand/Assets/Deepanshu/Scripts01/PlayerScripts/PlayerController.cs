using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Inputs _controls;
    private Vector2 _movement;
    
    [SerializeField] private Rigidbody _rb;
    public float moveSpeed;
    public CinemachineCamera cineCamera;
    
    [SerializeField] float shoulderChangeDistance =1.50f;
    [SerializeField] private Transform rightShoulder;
    [SerializeField] private Transform leftShoulder;
    
    [SerializeField] private bool isRightShoulder = true;
    
    private void Awake()
    {
        _controls = new Inputs();
        
        _controls.PlayerMovement.Movement.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Movement.canceled += ctx => _movement = Vector2.zero;
        _controls.PlayerMovement.ShoulderChange.performed += ctx => CamChange();
    }
    private void OnEnable()
    {
        _controls.Enable();
    }
    private void OnDisable()
    {
        _controls.Disable();
    }
    private void FixedUpdate()
    {
        Move();
        MaintainCameraFacingWorldZ();
    }
    private void Move()
    {
        Vector3 moveDirection = new Vector3(_movement.x, 0, _movement.y);
        moveDirection = transform.TransformDirection(moveDirection);
        _rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, _rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }
    private void MaintainCameraFacingWorldZ()
    {
        cineCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    private void CamChange()
    {
        isRightShoulder = !isRightShoulder;

        var cinemachineFollow = cineCamera.GetComponent<CinemachineFollow>();

        if (cinemachineFollow != null)
        {
            Vector3 newOffset = cinemachineFollow.FollowOffset;
            if (isRightShoulder)
            {
                newOffset.x = shoulderChangeDistance;
            }
            else
            {
                newOffset.x = -shoulderChangeDistance;
            }
            cinemachineFollow.FollowOffset = newOffset;
        }
    }
}
