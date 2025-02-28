using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CameraState
{
    Default,
    ADS
}
public class PlayerController : MonoBehaviour
{
    private Inputs _controls;
    private Vector2 _movement;  
    private Vector2 _lookDelta; 
    [SerializeField] public Transform shootPoint;
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody rb;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float yawClamp = 90f;  
    [SerializeField] private float pitchClamp = 80f; 
    private float baseYaw;
    private float yaw;
    private float pitch;

    [Header("Camera & ADS Settings")]
   
    [SerializeField] private Transform cameraPivot;  
    [SerializeField] private Camera playerCamera;      
    [SerializeField] private float cameraDamping = 5f; 
   
    [SerializeField] private Vector3 defaultCamLocalPos = new Vector3(0.5f, 1.5f, -3f);
    [SerializeField] private Vector3 adsCamLocalPos = new Vector3(0.5f, 1.5f, -1.5f);
    [SerializeField] private float defaultFOV = 90f;
    [SerializeField] private float adsFOV = 40f;

    [Header("Shoulder Switch Settings")]
  
    [SerializeField] private float shoulderOffset = 0.5f;
    private bool isRightShoulder = true;
    private bool isSwitchingShoulder = false;

    [Header("Camera State (State Machine)")]
    [SerializeField] private CameraState camState = CameraState.Default;


    [Header("Recoil Settings")]
    [SerializeField] private float recoilAmount = 0.2f;

    private Vector3 targetCamLocalPos;
    private void Awake()
    { 
        if (rb == null) rb = GetComponent<Rigidbody>();

     
        _controls = new Inputs();
        _controls.PlayerMovement.Movement.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Movement.canceled  += ctx => _movement = Vector2.zero;
        _controls.PlayerMovement.Look.performed+= ctx => _lookDelta = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Look.canceled+= ctx => _lookDelta = Vector2.zero;
        _controls.PlayerMovement.ADS.performed+= ctx => camState = CameraState.ADS;
        _controls.PlayerMovement.ADS.canceled += ctx => camState = CameraState.Default;
        _controls.PlayerMovement.ShoulderChange.performed += ctx => SwitchShoulder();
        
        baseYaw = transform.eulerAngles.y;
        yaw = baseYaw;
        pitch = 0f;

        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = defaultCamLocalPos;
            playerCamera.fieldOfView = defaultFOV;
            targetCamLocalPos = defaultCamLocalPos;
        }
    }
    private void OnEnable()  { _controls.Enable(); }
    private void OnDisable() { _controls.Disable(); }
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
      
        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
    private void UpdateCamera()
    {
        targetCamLocalPos = (camState == CameraState.ADS) ? adsCamLocalPos : defaultCamLocalPos;
       
        targetCamLocalPos.x = (isRightShoulder ? Mathf.Abs(targetCamLocalPos.x) : -Mathf.Abs(targetCamLocalPos.x))
                              + (isRightShoulder ? shoulderOffset : -shoulderOffset);
        
        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetCamLocalPos, Time.deltaTime * cameraDamping);
            float targetFOV = (camState == CameraState.ADS) ? adsFOV : defaultFOV;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * cameraDamping);
          
            Vector3 camEuler = playerCamera.transform.localEulerAngles;
            playerCamera.transform.localRotation = Quaternion.Euler(camEuler.x, 0, 0);
        }
    }
    private void SwitchShoulder()
    {
        if (isSwitchingShoulder) return;
        if (camState == CameraState.ADS) return;
        isRightShoulder = !isRightShoulder;
        StartCoroutine(ShoulderSwitchDelay(0.3f));
    }

    private IEnumerator ShoulderSwitchDelay(float delay)
    {
        isSwitchingShoulder = true;
        yield return new WaitForSeconds(delay);
        isSwitchingShoulder = false;
    }
    public void ApplyRecoil(Vector3 shootDirection)
    {
        transform.position -= shootDirection * recoilAmount;
    }

}

