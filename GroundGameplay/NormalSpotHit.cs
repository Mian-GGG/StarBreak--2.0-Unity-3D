using UnityEngine;

public class NormalSpotHit : MonoBehaviour, IDamageable
{
    public EnemyAI _enemyScript;
    void Start()
    {
        _enemyScript = transform.GetComponentInParent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage, bool isPenetrating = false)
    {
        _enemyScript.TakeDamage(5);
    }
}
