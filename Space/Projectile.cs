using UnityEngine;

public class Projectile : MonoBehaviour
{
    float _launchForce;
    int _damage;
    float _range;
    float _duration;
    Rigidbody _rigidBody;
    [SerializeField] Detonator _hitEffect;
    bool OutOfFuel
    {
        get
        {
            _duration -= Time.deltaTime;
            return _duration <= 0f;
        }
    }

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        _rigidBody.AddForce(_launchForce * transform.forward);
        _duration = _range;
    }

    void OnDisable()
    {
        _rigidBody.linearVelocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
    }

    void Update()
    {
        if (OutOfFuel) Destroy(gameObject);
    }

    public void Init(int launchForce, int damage, float range, Vector3 velocity, Vector3 angularVelocity)
    {
        _launchForce = launchForce;
        _damage = damage;
        _range = range;
        _rigidBody.linearVelocity = velocity;
        _rigidBody.angularVelocity = angularVelocity;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent<ISpaceDamageable>(out var damageable))
            damageable.TakeDamage(_damage, collider.ClosestPoint(transform.position));

        if (_hitEffect != null)
            Instantiate(_hitEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}