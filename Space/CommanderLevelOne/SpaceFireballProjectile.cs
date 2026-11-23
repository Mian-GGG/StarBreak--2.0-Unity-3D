using System.Collections;
using UnityEngine;

public class SpaceFireballProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 50f;
    [SerializeField] private float _lifetime = 2f;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private int _damage = 250;
    [SerializeField] private GameObject _explosionEffectPrefab;
    [SerializeField] private ParticleSystem _fireballEffect;

    private Rigidbody _rb;
    private Vector3 _direction;
    private bool _isExploded = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    public void Launch(Vector3 direction)
    {
        _direction = direction.normalized;
        StartCoroutine(LifetimeCoroutine());
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = _direction * _speed;
    }

    IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(_lifetime);
        Explode();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Commander")) return;
        Explode();
    }

    void Explode()
    {
        if (_isExploded) return;
        _isExploded = true;

        if (_explosionEffectPrefab != null)
            Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<ISpaceDamageable>(out var damageable))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float damagePercent = 1f - (distance / _explosionRadius);
                int finalDamage = Mathf.RoundToInt(_damage * damagePercent);
                damageable.TakeDamage(finalDamage, hit.transform.position);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}