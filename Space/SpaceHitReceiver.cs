using System.Collections.Generic;
using UnityEngine;

public class SpaceHitReceiver : MonoBehaviour, ISpaceDamageable
{
    [SerializeField] GameObject[] _damageDelegates;
    List<ISpaceDamageable> _damageReceivers;

    void Awake()
    {
        if (_damageDelegates == null) return;
        _damageReceivers = new List<ISpaceDamageable>();
        foreach (GameObject damageDelegate in _damageDelegates)
        {
            if (damageDelegate.TryGetComponent<ISpaceDamageable>(out var damageable))
                _damageReceivers.Add(damageable);
        }
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        if (_damageReceivers == null) return;
        foreach (ISpaceDamageable damageable in _damageReceivers)
            damageable.TakeDamage(damage, hitPosition);
    }
}