using System;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float _speed = 3f;
    private float _lifetime = 10f;
    public int damage = 10;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("No Rigidbody found on bullet!", this);
        }
    }
    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
        _rb.linearVelocity = transform.forward * _speed;
        Invoke(nameof(DestroyBullet), _lifetime);
    }
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            DestroyBullet();
            return;
        }

        IObstacle obstacle = other.GetComponent<IObstacle>();
        if (obstacle != null)
        {
            Obstacle specificObstacle = other.GetComponent<Obstacle>();
            if (specificObstacle != null)
            {
                specificObstacle.OnBulletHit();
            }
            DestroyBullet();
        }
    }
    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
