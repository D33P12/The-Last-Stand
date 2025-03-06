using UnityEngine;

public class RefillMaxAmmoCollectable : MonoBehaviour, ICollectable
{
    [SerializeField] private int ammoAmount = 30;
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
