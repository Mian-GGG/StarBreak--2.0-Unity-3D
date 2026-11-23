using UnityEngine;

public class SpaceAsteroid : MonoBehaviour, ISpaceDamageable
{
    [SerializeField] private GameObject _fracturedAsteroidPrefab;
    [SerializeField] private Detonator _explosionPrefab;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        FractureAsteroid();
    }

    private void FractureAsteroid()
    {
        if (_fracturedAsteroidPrefab != null)
        {
            GameObject fractured = Instantiate(
                _fracturedAsteroidPrefab,
                _transform.position,
                _transform.rotation);

            fractured.transform.localScale = _transform.localScale;
        }

        if (_explosionPrefab != null)
            Instantiate(_explosionPrefab, _transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}