using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float speed = 10f;
    private float lifetime = 40f;
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
            ReturnToPool();
        }
    }
}
