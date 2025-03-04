using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private int bulletDamage = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable enemy))
        {
            enemy.TakeDamage(bulletDamage);
            gameObject.SetActive(false); 
        }
    }
}
