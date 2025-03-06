using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float speed = 3f;
    private float lifetime = 10f; 
    public int damage = 10; 

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        Invoke(nameof(ReturnToPool), lifetime);
    }
    private void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }
    private void ReturnToPool()
    {
        BulletPool.Instance.ReturnBullet(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            ReturnToPool();
        }
    }
}
