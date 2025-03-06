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

        if (other.TryGetComponent<ICollectable>(out ICollectable collectible))
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                collectible.Collect(player);
            }

            Destroy(gameObject); 
        }
    }
}
