using UnityEngine;
public class Cover : MonoBehaviour,IInteractable
{ 
    public void Interact(PlayerController player)
    {
        player.ToggleCover(this.transform.position);
    }
}
