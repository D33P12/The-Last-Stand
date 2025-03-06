using UnityEngine;

public class HealthCollectable : MonoBehaviour, ICollectable
{
    [SerializeField] private int healthAmount = 20; 

    public void Collect(PlayerController player)
    {
        if (player != null) 
        {
            player.Heal(healthAmount);
            Destroy(gameObject);
        }
    }
}
