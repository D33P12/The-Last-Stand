using UnityEngine;

public class HealthCollectable : MonoBehaviour, ICollectable
{
    [SerializeField] private int healthAmount = 20; 
    [SerializeField] private float lifetime = 5f;
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    public void Collect(PlayerController player)
    {
        if (player != null) 
        {
            player.Heal(healthAmount);
            Destroy(gameObject);
        }
    }
}
