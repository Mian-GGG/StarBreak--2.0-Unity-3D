using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommanderShip : MonoBehaviour
{
    [Header("Weak Spots")]
    [SerializeField] private CommanderWeakSpot[] _weakSpots;

    [Header("Shield")]
    [SerializeField] private CommanderShield _shield;

    [Header("Fireball Attack")]
    [SerializeField] private Transform[] _fireballSpawnPoints;
    [SerializeField] private SpaceFireballProjectile _fireballPrefab;
    [SerializeField] private float _fireballCooldown = 3f;
    [SerializeField] private float _fireballCooldownIntensified = 1f;

    [Header("Laser Turrets")]
    [SerializeField] private LaserTurret[] _laserTurrets;
    [SerializeField] private float _laserDuration = 2f;
    [SerializeField] private float _laserCooldown = 5f;
    [SerializeField] private float _targetUpdateDelay = 0.5f;

    [Header("Spawning")]
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private int _totalEnemies = 15;
    [SerializeField] public int _totalWaves = 3;
    [SerializeField] private float _spawnRadius = 200f;
    [SerializeField] private float _waveCooldown = 10f;
    [SerializeField] private float _minSpawnRadius = 300f;
    [SerializeField] private float _maxSpawnRadius = 500f;

    [Header("Death")]
    [SerializeField] private GameObject _explosionPrefab;

    [Header("About the Commander")]
    [SerializeField] private bool isLevelOne = true;

    public UnityEvent OnCommanderDestroyed;

    private Transform _player;
    public List<GameObject> _activeEnemies = new List<GameObject>();
    public bool _enemiesAlive = true;
    private bool _isLaserActive = false;
    private float _nextFireballTime = 0f;
    private float _nextLaserTime = 0f;
    private int _weakSpotsDestroyed = 0;
    private int _enemiesPerWave;
    public int _wavesSpawned = 0;
    public int enemyCount;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _enemiesPerWave = Mathf.FloorToInt((float)_totalEnemies / _totalWaves);

        foreach (var weakSpot in _weakSpots)
            weakSpot.OnWeakSpotDestroyed.AddListener(OnWeakSpotDestroyed);

        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        if (_player == null) return;

        CheckEnemiesAlive();
        HandleFireballAttack();

        if (!_enemiesAlive)
            HandleLaserAttack();


        _activeEnemies.RemoveAll(e => e == null || !e.activeSelf);
        enemyCount = _activeEnemies.Count;
    }

    // ===================== SPAWNING =====================

    IEnumerator SpawnWaves()
    {
        while (_wavesSpawned < _totalWaves)
        {
            SpawnWave();
            _wavesSpawned++;
            yield return new WaitForSeconds(_waveCooldown);
            yield return new WaitUntil(() => AllWaveEnemiesDead());
        }
    }

    [Header("Cleanup")]
    [SerializeField] private Transform _enemyContainer;

    void SpawnWave()
    {
        for (int i = 0; i < _enemiesPerWave; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere.normalized;
            float randomDistance = Random.Range(_minSpawnRadius, _maxSpawnRadius);
            Vector3 spawnPos = transform.position + randomDirection * randomDistance;

            GameObject prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, spawnPos,
                Quaternion.identity, _enemyContainer);

            enemy.GetComponent<EnemySpaceShipController>()
                .ShipDestroyed.AddListener(OnEnemyDestroyed);

            enemy.SetActive(true);
            _activeEnemies.Add(enemy);
        }
    }

    void OnEnemyDestroyed(int id)
    {
        _activeEnemies.RemoveAll(e => e == null ||
            e.GetInstanceID() == id || !e.activeSelf);

        Debug.Log($"Enemy destroyed. Remaining: {_activeEnemies.Count}");
        enemyCount = _activeEnemies.Count;
    }

    bool AllWaveEnemiesDead()
    {
        _activeEnemies.RemoveAll(e => e == null || !e.activeSelf);
        return _activeEnemies.Count == 0;
    }

    // ===================== WEAK SPOTS =====================

    void OnWeakSpotDestroyed()
    {
        _weakSpotsDestroyed++;
        if (_weakSpotsDestroyed >= _weakSpots.Length)
            DestroyCommander();
    }

    // ===================== SHIELD =====================

    void CheckEnemiesAlive()
    {
        if (!_enemiesAlive) return;

        _activeEnemies.RemoveAll(e => e == null || !e.activeSelf);

        if(_wavesSpawned == _totalWaves)
        {
            Debug.Log("All waves happened" + "\n" + "Wavesspawned: " + _wavesSpawned + "  TotalWaves: " + _totalWaves);
            if (_activeEnemies.Count == 0)
            {
                _enemiesAlive = false;
                if (_shield != null)
                    _shield.BreakShield();

                foreach (var weakSpot in _weakSpots)
                {
                    if (!weakSpot.IsDestroyed)
                        weakSpot.RegisterWithUI();
                }
            }
        }
    }


    // ===================== FIREBALL =====================

    void HandleFireballAttack()
    {
        if (Time.time < _nextFireballTime) return;

        CheckEnemiesAlive();

        float cooldown = _enemiesAlive ?
            _fireballCooldown : _fireballCooldownIntensified;

        FireFireball();
        _nextFireballTime = Time.time + cooldown;
    }

    [SerializeField] private Transform _fireballContainer;

    void FireFireball()
    {
        if (_fireballPrefab == null) return;

        foreach (var spawnPoint in _fireballSpawnPoints)
        {
            SpaceFireballProjectile fireball = Instantiate(
                _fireballPrefab,
                spawnPoint.position,
                spawnPoint.rotation,
                _fireballContainer);

            fireball.Launch(spawnPoint.forward);
        }
    }

    // ===================== LASER =====================

    void HandleLaserAttack()
    {
        if (_isLaserActive) return;
        if (Time.time < _nextLaserTime) return;
        StartCoroutine(LaserCoroutine());
    }

    IEnumerator LaserCoroutine()
    {
        _isLaserActive = true;

        foreach (var turret in _laserTurrets)
            turret.ActivateLaser();

        float elapsed = 0f;
        float targetUpdateTimer = 0f;

        Vector3 storedPlayerPosition = _player.position;

        while (elapsed < _laserDuration)
        {
            elapsed += Time.deltaTime;
            targetUpdateTimer += Time.deltaTime;
            if (targetUpdateTimer >= _targetUpdateDelay)
            {
                storedPlayerPosition = _player.position;
                targetUpdateTimer = 0f;
            }

            foreach (var turret in _laserTurrets)
                turret.SetTarget(storedPlayerPosition);

            yield return null;
        }

        foreach (var turret in _laserTurrets)
            turret.DeactivateLaser();

        _isLaserActive = false;
        _nextLaserTime = Time.time + _laserCooldown;
    }

    // ===================== DEATH =====================

    void DestroyCommander()
    {
        if (_explosionPrefab != null)
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        OnCommanderDestroyed.Invoke();

        if (isLevelOne)
        {
            GameObject.Find("Player").GetComponent<InteractionManager>().canEnterPlanet = true;
        }


        
        gameObject.SetActive(false);
    }
}