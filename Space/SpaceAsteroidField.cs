using System.Collections.Generic;
using UnityEngine;

public class SpaceAsteroidField : MonoBehaviour
{
    [Header("Field Settings")]
    [SerializeField][Range(100, 3000)] private int _asteroidCount = 500;
    [SerializeField][Range(100f, 3000f)] private float _radius = 300f;
    [SerializeField][Range(1f, 10f)] private float _maxScale = 5f;

    [Header("Asteroid Prefabs")]
    [SerializeField] private List<GameObject> _asteroidPrefabs;

    public float Radius => _radius;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void OnEnable()
    {
        SpawnAsteroids();
    }

    [SerializeField] private Transform _asteroidContainer;

    void SpawnAsteroids()
    {
        for (int i = 0; i < _asteroidCount; i++)
        {
            GameObject prefab = _asteroidPrefabs[Random.Range(0, _asteroidPrefabs.Count)];
            GameObject asteroid = Instantiate(prefab, _transform.position,
                Quaternion.identity, _asteroidContainer);

            float scale = Random.Range(0.5f, _maxScale);
            asteroid.transform.localScale = Vector3.one * scale;
            asteroid.transform.position += Random.insideUnitSphere * _radius;

            Rigidbody rb = asteroid.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddTorque(Random.insideUnitSphere *
                    Random.Range(0f, 50f), ForceMode.Impulse);
        }
    }
}