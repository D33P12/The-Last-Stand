using UnityEngine;

public class Obstacle : MonoBehaviour,IObstacle
{
    [Header("Bullet Hit Settings")]
    [SerializeField] private int maxHits = 3; 
    [SerializeField] private float cooldownTime = 5f;

    [Header("Visual Feedback")]
    [SerializeField] private Renderer obstacleRenderer;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color disabledColor = Color.red;

    private int _currentHits = 0;
    private Collider _obstacleCollider;

    private void Start()
    {
        _obstacleCollider = GetComponent<Collider>();

        if (_obstacleCollider == null)
        {
            Debug.LogError("No Collider found on the obstacle!", this);
        }
        if (obstacleRenderer != null)
        {
            obstacleRenderer.material.color = activeColor;
        }
    }
    public void OnBulletHit()
    {
        _currentHits++;

        if (_currentHits >= maxHits)
        {
            _obstacleCollider.enabled = false;
            if (obstacleRenderer != null)
            {
                obstacleRenderer.material.color = disabledColor;
            }
            StartCoroutine(Cooldown());
        }
    }
    private System.Collections.IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        _obstacleCollider.enabled = true;
        _currentHits = 0;
        if (obstacleRenderer != null)
        {
            obstacleRenderer.material.color = activeColor;
        }
    }
}
