using System.Collections.Generic;
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
    
    [SerializeField] private bool isRightShoulder = true;
    
    public Transform shootPoint;
    public GameObject bulletPrefab;
    private Queue<GameObject> bulletPool = new Queue<GameObject>();
    public int poolSize = 10;
    public float bulletSpeed = 20f;
    public float recoilAmount = 2f;
    
    private void Awake()
    {
        _controls = new Inputs();
        
        _controls.PlayerMovement.Movement.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Movement.canceled += ctx => _movement = Vector2.zero;
        _controls.PlayerMovement.ShoulderChange.performed += ctx => CamChange();
        _controls.PlayerMovement.Shoot.performed += ctx => Shoot();
        
        InitializeBulletPool();
        
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
    }
    private void Move()
    {
        Vector3 moveDirection = new Vector3(_movement.x, 0, _movement.y);
        moveDirection = transform.TransformDirection(moveDirection);
        _rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, _rb.linearVelocity.y, moveDirection.z * moveSpeed);
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
    private void Shoot()
    {
        RaycastHit hit;
        Vector3 shootDirection;

        if (Physics.Raycast(cineCamera.transform.position, cineCamera.transform.forward, out hit, 100f))
        {
            shootDirection = (hit.point - shootPoint.position).normalized; 
        }
        else
        {
            shootDirection = cineCamera.transform.forward;
        }
        FirePooledBullet(shootDirection); 
        transform.position -= shootDirection * recoilAmount * Time.deltaTime;
    }
    private void FirePooledBullet(Vector3 shootDirection)
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.transform.position = shootPoint.position; 
            bullet.transform.rotation = Quaternion.LookRotation(shootDirection); 
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().linearVelocity = shootDirection * bulletSpeed;

            StartCoroutine(ReturnBulletToPool(bullet, 2f));
        }
    }
    private System.Collections.IEnumerator ReturnBulletToPool(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
    private void InitializeBulletPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }
}
