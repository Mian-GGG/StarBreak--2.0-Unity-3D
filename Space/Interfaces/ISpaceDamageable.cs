using UnityEngine;

public interface ISpaceDamageable
{
    void TakeDamage(int damage, Vector3 hitPosition);
}