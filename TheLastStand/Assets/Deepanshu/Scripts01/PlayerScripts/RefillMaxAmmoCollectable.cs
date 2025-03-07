using UnityEngine;

public class RefillMaxAmmoCollectable : MonoBehaviour, ICollectable
{
    [SerializeField] private int ammoAmount = 30;
    [SerializeField] private float lifetime = 5f;
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    public void Collect(PlayerController player)
    {
        if (player != null)
        {
            ShootiController shootController = player.GetComponent<ShootiController>();
            if (shootController != null)
            {
                shootController.RefillMaxAmmo(ammoAmount);
                Destroy(gameObject);
            }
        }
    }
}
