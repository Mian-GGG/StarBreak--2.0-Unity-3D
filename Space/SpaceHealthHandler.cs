using UnityEngine;
using UnityEngine.Events;

public class SpaceHealthHandler : MonoBehaviour, ISpaceDamageable
{
    [SerializeField] GameObject _explosionPrefab;

    UnityEvent _healthChangedEvent;
    UnityEvent _objectDestroyedEvent;

    public int MaxHealth { get; private set; }
    public int Health { get; private set; }

    public UnityEvent HealthChanged => _healthChangedEvent ??= new UnityEvent();
    public UnityEvent ObjectDestroyed => _objectDestroyedEvent ??= new UnityEvent();

    public void Init(int maxHealth)
    {
        Health = MaxHealth = maxHealth;
        HealthChanged.Invoke();
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        Health -= damage;
        //Debug.Log($"Player took {damage} damage. Health now: {Health}");
        HealthChanged.Invoke();
        if (Health > 0) return;
        if (_explosionPrefab)
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        ObjectDestroyed.Invoke();
    }

    public void Heal(int amount)
    {
        Health = Mathf.Min(Health + amount, MaxHealth);
        HealthChanged.Invoke();
    }
}