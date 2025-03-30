using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField]
    private float explosionRadius = 5f;
    [SerializeField]
    private float explosionDamage = 50f;
    [SerializeField]
    private float timerDuration = 5f;
    [SerializeField]
    private LayerMask groundLayer;

    private float timer;
    private bool hasLanded = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (hasLanded)
        {
            timer += Time.deltaTime;
            if (timer >= timerDuration)
            {
                Explode();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && !hasLanded)
        {
            hasLanded = true;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    public void Launch(Vector3 velocity)
    {
        rb.linearVelocity = velocity;
    }
    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            IDamageable damageable = nearbyObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage((int)explosionDamage);
            }
        }
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
