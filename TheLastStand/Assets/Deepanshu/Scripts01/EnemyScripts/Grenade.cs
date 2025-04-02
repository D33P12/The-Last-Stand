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

    private float _timer;
    private bool _hasLanded = false;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (_hasLanded)
        {
            _timer += Time.deltaTime;
            if (_timer >= timerDuration)
            {
                Explode();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && !_hasLanded)
        {
            _hasLanded = true;
            _rb.isKinematic = true;
            _rb.useGravity = false;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
    public void Launch(Vector3 velocity)
    {
        _rb.linearVelocity = velocity;
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
