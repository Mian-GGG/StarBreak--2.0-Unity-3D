using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.UI.Image;

public class FireBallProjectile : MonoBehaviour, IExplodeable
{
    public bool rbOn = false, isExploded = false, isThrown = false, damagedOnce = false, fireEffectOnce = false, explosionEffectOnce = false;
    public float explosionRadius = 1f;
    public GameObject owner;
    public ParticleSystem fireBallEffect, ExplosionEffect, fireEffect;
    private Rigidbody rb;
    private Vector3 explosionPos;
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rbOn)
        {
            gameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        if(!isThrown && owner.GetComponent<EnemyAI>().isDead)
        {
            Explode();
        }
    }

    void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.transform.root.gameObject == owner || other.gameObject == fireBallEffect.gameObject || other.tag == "Explodable")
        {
            return;
        }
        else
        {
            Exploder();
            StartCoroutine(ExplosionSystem());
        }
    }

    void Exploder()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        explosionPos = gameObject.transform.position;
        foreach (Collider col in hits)
        {
            if (col.gameObject == owner && !isExploded)
            {
                return;
            }
            else
            {
                Explosion(col);
            }
        }
    }

    void Explosion(Collider collider)
    {
        if (damagedOnce)
        {
            return;
        }
        if (!explosionEffectOnce)
        {
            Instantiate(ExplosionEffect, explosionPos, Quaternion.identity);
            explosionEffectOnce = true;
        }
        
        float distance = Vector3.Distance(transform.position, collider.transform.position);
        float damagePercent = 1 - (distance / explosionRadius);
        float finalDamage = 30 * damagePercent;

        if (collider.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(finalDamage);
            damagedOnce = true;
        }
        else if(collider.gameObject.tag == "Floor" && !fireEffectOnce)
        {
            Instantiate(fireEffect, explosionPos, Quaternion.identity);
            fireEffectOnce = true;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public void Explode()
    {
        isExploded = true;
        Exploder();
        StartCoroutine(ExplosionSystem());
    }

    IEnumerator ExplosionSystem()
    {
        rb.isKinematic = true;
        ExplosionEffect.Play();
        yield return new WaitForSecondsRealtime(1f);
        Destroy(gameObject);
    }
}
