using UnityEngine;
using UnityEngine.Events;

public class CommanderWeakSpot : MonoBehaviour, ISpaceDamageable
{
    [SerializeField] private int _maxHealth = 1000;
    [SerializeField] private GameObject _destructionEffect;

    public bool IsDestroyed { get; private set; }
    public UnityEvent OnWeakSpotDestroyed = new UnityEvent();

    private int _health;
    private bool _isRegistered = false;

    void OnEnable()
    {
        _health = _maxHealth;
        IsDestroyed = false;
    }

    void OnDisable()
    {
        UnregisterFromUI();
    }

    void OnDestroy()
    {
        UnregisterFromUI();
    }

    public void RegisterWithUI()
    {
        if (_isRegistered) return;
        if (UIManager.Instance == null) return;
        UIManager.Instance.AddTarget(transform);
        _isRegistered = true;
    }

    void UnregisterFromUI()
    {
        if (!_isRegistered) return;
        if (UIManager.Instance == null) return;
        UIManager.Instance.RemoveTarget(transform);
        _isRegistered = false;
    }

    public UnityEvent OnHealthChanged = new UnityEvent();

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        if (IsDestroyed) return;

        _health -= damage;

        if (DamageNumberSpawner.Instance != null)
            DamageNumberSpawner.Instance.Spawn(damage, hitPosition);

        OnHealthChanged.Invoke();

        if (_health <= 0)
            DestroyWeakSpot();
    }

    void DestroyWeakSpot()
    {
        IsDestroyed = true;

        if (_destructionEffect != null)
            Instantiate(_destructionEffect, transform.position, Quaternion.identity);

        OnWeakSpotDestroyed.Invoke();
        gameObject.SetActive(false);
    }



    public int Health => _health;
    public int MaxHealth => _maxHealth;
}