using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceShipController : MonoBehaviour
{
    [SerializeField] protected MovementControlsBase _movementControls;
    [SerializeField] protected WeaponControlsBase _weaponControls;
    [SerializeField] SpaceShipDataSO _shipData;
    [SerializeField] List<SpaceShipEngine> _engines;
    [SerializeField] List<Blaster> _blasters;

    Rigidbody _rigidBody;
    float _pitchAmount, _rollAmount, _yawAmount;

    protected SpaceHealthHandler _healthHandler;

    IMovementControls MovementInput => _movementControls;
    IWeaponControls WeaponInput => _weaponControls;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _healthHandler = GetComponent<SpaceHealthHandler>();
    }

    void Start()
    {
        foreach (SpaceShipEngine engine in _engines)
            engine.Init(MovementInput, _rigidBody, _shipData.ThrustForce / _engines.Count);

        foreach (Blaster blaster in _blasters)
            blaster.Init(
                WeaponInput,
                _shipData.BlasterCooldown,
                _shipData.BlasterLaunchForce,
                _shipData.BlasterProjectileDuration,
                _shipData.BlasterDamage,
                _rigidBody,
                _shipData.BlasterMaxCapacitor,
                _shipData.BlasterCostPerShot,
                _shipData.BlasterRechargeRate);
    }

    public virtual void OnEnable()
    {
        if (_healthHandler == null) return;
        _healthHandler.Init(_shipData.MaxHealth);
        _healthHandler.HealthChanged.AddListener(OnHealthChanged);
        _healthHandler.ObjectDestroyed.AddListener(DestroyShip);
    }

    public virtual void Update()
    {
        _rollAmount = MovementInput.RollAmount;
        _yawAmount = MovementInput.YawAmount;
        _pitchAmount = MovementInput.PitchAmount;
    }

    void FixedUpdate()
    {
        if (!Mathf.Approximately(0f, _pitchAmount))
            _rigidBody.AddTorque(transform.right *
                (_shipData.PitchForce * _pitchAmount * Time.fixedDeltaTime));

        if (!Mathf.Approximately(0f, _rollAmount))
            _rigidBody.AddTorque(transform.forward *
                (_shipData.RollForce * _rollAmount * Time.fixedDeltaTime));

        if (!Mathf.Approximately(0f, _yawAmount))
            _rigidBody.AddTorque(transform.up *
                (_yawAmount * _shipData.YawForce * Time.fixedDeltaTime));
    }

    void OnHealthChanged()
    {
    }

    [SerializeField] private bool _isPlayer = false;
    [SerializeField] protected GameObject _healthPickupPrefab;
    [SerializeField] protected Transform _pickupContainer;

    void DestroyShip()
    {
        if (_healthPickupPrefab != null && !_isPlayer)
            Instantiate(_healthPickupPrefab, transform.position,
                Quaternion.identity);

        gameObject.SetActive(false);

        if (_isPlayer)
        {
            PlayerPrefs.SetString("LastScene",
                SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();
            SceneManager.LoadScene("DeathScene");
        }
    }
}