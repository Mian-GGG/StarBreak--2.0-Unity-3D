using UnityEngine;

[CreateAssetMenu(fileName = "ShipData", menuName = "StarBreak/Ship Data", order = 1)]
public class SpaceShipDataSO : ScriptableObject
{
    [SerializeField]
    [Range(1000f, 10000f)]
    float _thrustForce = 7500f,
        _pitchForce = 6000f,
        _rollForce = 1000f,
        _yawForce = 2000f;

    [SerializeField]
    private int _shieldStrength = 5000,
        _shieldRegenAmount = 100,
        _maxHealth = 5000;

    [SerializeField]
    [Range(10, 1000)]
    int _blasterDamage = 100;

    [Range(5000f, 25000f)]
    [SerializeField]
    int _blasterLaunchForce = 10000;

    [SerializeField]
    float _blasterCoolDown = 0.25f;

    [SerializeField]
    [Range(2f, 10f)]
    float _blasterProjectileDuration = 2f;


    [Header("Blaster Capacitor")]
    [SerializeField] private float _blasterMaxCapacitor = 1000f;
    [SerializeField] private float _blasterCostPerShot = 50f;
    [SerializeField] private float _blasterRechargeRate = 20f;

    public float BlasterMaxCapacitor => _blasterMaxCapacitor;
    public float BlasterCostPerShot => _blasterCostPerShot;
    public float BlasterRechargeRate => _blasterRechargeRate;

    public float ThrustForce => _thrustForce;
    public float PitchForce => _pitchForce;
    public float RollForce => _rollForce;
    public float YawForce => _yawForce;
    public int ShieldStrength => _shieldStrength;
    public int ShieldRegenAmount => _shieldRegenAmount;
    public int MaxHealth => _maxHealth;
    public int BlasterLaunchForce => _blasterLaunchForce;
    public int BlasterDamage => _blasterDamage;
    public float BlasterProjectileDuration => _blasterProjectileDuration;
    public float BlasterCooldown => _blasterCoolDown;
}