using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootiController : MonoBehaviour
{
    private Inputs _controls;
    private bool isShooting = false;
    [SerializeField] private float shootCooldown = 0.1f;
    private float lastShootTime = 0f;

    [Header("Shooting References")] [SerializeField]
    private Camera shootCamera;

    [SerializeField] private Transform shootPoint;

    [Header("Shooting Settings")] [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private int poolSize = 10;
    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    [Header("Recoil (Shooting)")] [SerializeField]
    private float recoilAmount = 0.2f;

    private PlayerController playerController;
    private void Awake()
    {
        _controls = new Inputs();
        _controls.PlayerMovement.Shoot.started += ctx => isShooting = true;
        _controls.PlayerMovement.Shoot.canceled += ctx => isShooting = false;
        InitializeBulletPool();
        playerController = GetComponent<PlayerController>();
    }
    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Update()
    {
        if (isShooting && Time.time - lastShootTime >= shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }
    private void Shoot()
    {
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray camRay = shootCamera.ScreenPointToRay(screenCenter);

        Vector3 targetPoint;
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = camRay.origin + camRay.direction * 100f; 
        }

        Vector3 shootDir = (targetPoint - shootPoint.position).normalized;

        GameObject bullet = GetBulletFromPool();
        if (bullet != null)
        {
            bullet.transform.position = shootPoint.position;
            bullet.transform.rotation = Quaternion.LookRotation(shootDir);
            bullet.SetActive(true);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = shootDir * bulletSpeed;

            StartCoroutine(ReturnBulletToPool(bullet, 2f));
        }

        if (playerController != null)
            playerController.ApplyRecoil(shootDir);
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
    private GameObject GetBulletFromPool()
    {
        if (bulletPool.Count > 0)
        {
            return bulletPool.Dequeue();
        }
        else
        {
           
            return Instantiate(bulletPrefab);
        }
    }
    private IEnumerator ReturnBulletToPool(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}
