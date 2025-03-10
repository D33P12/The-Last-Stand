using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class ShootiController : MonoBehaviour
{ 
   private Inputs _controls;
    private bool _isShooting = false;
    private bool _isReloading = false;
  
    private float _lastShootTime = 0f;

    [Header("Shooting References")]
    [SerializeField] internal Camera shootCamera;
    [SerializeField] private Transform shootPoint;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private int poolSize = 10;
    private Queue<GameObject> _bulletPool = new Queue<GameObject>();

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int maxCarryingAmmo = 120;
    private int _currentAmmo;
    private int _carryingAmmo;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadText;

    [Header("Reload Settings")]
    [SerializeField] private float reloadTime = 2f;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilAmount = 2f;
    [SerializeField] private float recoilRecoverySpeed = 5f;
    [SerializeField] private float upwardRecoilRotationAmount = 5f;

    [Header("Cover System")]
    [SerializeField] private bool _isInCover = false; 
    [SerializeField] private Transform leftCoverPoint;
    [SerializeField] private Transform rightCoverPoint;
    [SerializeField] private LayerMask coverLayer;

    private bool _canShoot = true;
    private Quaternion _originalCameraRotation;
    private bool _isRecoiling = false;
    
    public bool IsShooting => _isShooting;

    private void Awake()
    {
        _controls = new Inputs();
        _controls.PlayerMovement.Shoot.started += ctx => TryShoot();
        _controls.PlayerMovement.Reload.started += ctx => Reload();
       
        InitializeBulletPool();
        _currentAmmo = maxAmmo;
        _carryingAmmo = maxCarryingAmmo;

        if (shootCamera != null)
        {
            _originalCameraRotation = shootCamera.transform.localRotation;
        }
    }
    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
    private void Update()
    {
        if (_isRecoiling)
        {
            shootCamera.transform.localRotation = Quaternion.Lerp(shootCamera.transform.localRotation, _originalCameraRotation, Time.deltaTime * recoilRecoverySpeed);
        }
        UpdateAmmoDisplay();
    }
    private void TryShoot()
    {
        if (!_isInCover || IsTouchingCoverPoint())
        {
            Shoot();
        }
    }
    private bool IsTouchingCoverPoint()
    {
        return Physics.CheckSphere(leftCoverPoint.position, 0.2f, coverLayer) ||
               Physics.CheckSphere(rightCoverPoint.position, 0.2f, coverLayer);
    }
    private void Shoot()
    {
        if (!_canShoot || _isReloading || _currentAmmo <= 0 || Time.time - _lastShootTime < 0.1f) return;

        _isShooting = true;
        _lastShootTime = Time.time;

        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray camRay = shootCamera.ScreenPointToRay(screenCenter);
        Vector3 targetPoint;

        if (Physics.Raycast(camRay, out RaycastHit hit, 100f))
            targetPoint = hit.point;
        else
            targetPoint = camRay.origin + camRay.direction * 100f;

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
        ApplyRecoil();
        _currentAmmo--;
    }
    private void ApplyRecoil()
    {
        if (shootCamera == null) return;

        Vector3 recoilOffset = new Vector3(
            Random.Range(-recoilAmount, recoilAmount) * 0.1f,
            Random.Range(-recoilAmount, recoilAmount) * 0.1f,
            0
        );

        Quaternion upwardRecoil = Quaternion.Euler(upwardRecoilRotationAmount, 0, 0);
        shootCamera.transform.localRotation *= upwardRecoil;
        shootCamera.transform.localPosition += recoilOffset;
        _isRecoiling = true;
    }
    private void InitializeBulletPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            _bulletPool.Enqueue(bullet);
        }
    }
    private GameObject GetBulletFromPool()
    {
        if (_bulletPool.Count > 0)
        {
            GameObject bullet = _bulletPool.Dequeue();
            return bullet;
        }
        return Instantiate(bulletPrefab);
    }
    private IEnumerator ReturnBulletToPool(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bullet.SetActive(false);
        _bulletPool.Enqueue(bullet);
    }
    private void Reload()
    {
        if (_isReloading || _currentAmmo == maxAmmo || _carryingAmmo == 0) return;
        StartCoroutine(ReloadCoroutine());
    }
    private IEnumerator ReloadCoroutine()
    {
        _isReloading = true;
        reloadText.text = "Reloading...";
        yield return new WaitForSeconds(reloadTime);
        int ammoToReload = Mathf.Min(maxAmmo - _currentAmmo, _carryingAmmo);
        _currentAmmo += ammoToReload;
        _carryingAmmo -= ammoToReload;

        reloadText.text = "";
        _isReloading = false;
    }
    public void SetCanShoot(bool canShoot)
    {
        _canShoot = canShoot;
    }
    private void UpdateAmmoDisplay()
    {
        if (ammoText != null)
            ammoText.text = $"{_currentAmmo}/{_carryingAmmo}";
    }
    public void RefillMaxAmmo(int amount)
    {
        int ammoToAdd = Mathf.Min(amount, maxCarryingAmmo - _carryingAmmo);
        _carryingAmmo += ammoToAdd;
        UpdateAmmoDisplay();
    }
    public void SetCoverState(bool isInCover)
    {
        _isInCover = isInCover;
    }
}