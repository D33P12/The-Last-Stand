using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;
    public GameObject bulletPrefab;
    public int poolSize = 10;
    private Queue<GameObject> _bulletPool = new Queue<GameObject>();
    private void Awake()
    {
        Instance = this;
        InitializePool();
    }
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            _bulletPool.Enqueue(bullet);
        }
    }
    public GameObject GetBullet()
    {
        GameObject bullet;
        if (_bulletPool.Count > 0)
        {
            bullet = _bulletPool.Dequeue();
        }
        else
        {
            bullet = Instantiate(bulletPrefab);
        }
        bullet.SetActive(true);
        bullet.transform.position = Vector3.zero;
        bullet.transform.rotation = Quaternion.identity;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        return bullet;
    }
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        _bulletPool.Enqueue(bullet);
    }
}
