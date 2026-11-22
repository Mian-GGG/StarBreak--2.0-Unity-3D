using UnityEngine;

public class WeakSpotHit : MonoBehaviour, IDamageable
{
    public EnemyAI _enemyScript;
    void Start()
    {
        _enemyScript = transform.GetComponentInParent<EnemyAI>();
    }

    void Update()
    {

    }

    public void TakeDamage(float damage, bool isPenetrating = false)
    {
        _enemyScript.TakeDamage(20);
    }
}
