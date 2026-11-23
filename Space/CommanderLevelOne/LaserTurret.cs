using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [Header("Turret Parts")]
    [SerializeField] private Transform _turretBase;
    [SerializeField] private Transform _turretCannon;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotateSpeed = 45f;
    [SerializeField] private float _minCannonAngle = 0f;
    [SerializeField] private float _maxCannonAngle = 180f;

    [Header("Laser")]
    [SerializeField] private ParticleSystem _laserParticle;
    [SerializeField] private ParticleSystem _laserBeam;
    [SerializeField] private float _laserRange = 500f;
    [SerializeField] private float _laserDamagePerTick = 150f;

    private Vector3 _targetPosition;
    private bool _isActive = false;
    private Transform _player;
    private float _currentCannonAngle = 360f;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _targetPosition = transform.position + transform.forward;
    }

    void Update()
    {
        if (!_isActive)
            TrackPlayer();

        RotateBase();
        RotateCannon();

        if (_isActive)
            FireLaser();
    }

    void TrackPlayer()
    {
        if (_player == null) return;

        Vector3 dirToPlayer = (_player.position - _turretBase.position).normalized;
        float dot = Vector3.Dot(_turretBase.forward, dirToPlayer);
        if (dot > 0f)
            _targetPosition = _player.position;
    }

    public void SetTarget(Vector3 position)
    {
        _targetPosition = position;
    }

    public void ActivateLaser()
    {
        _isActive = true;
        if (_laserParticle != null) _laserParticle.Play(); _laserParticle.gameObject.SetActive(true);
        if (_laserBeam != null) _laserBeam.Play();_laserBeam.gameObject.SetActive(true);
    }

    public void DeactivateLaser()
    {
        _isActive = false;
        if (_laserParticle != null) _laserParticle.Stop(); _laserParticle.gameObject.SetActive(false);
        if (_laserBeam != null) _laserBeam.Stop();_laserBeam.gameObject.SetActive(false);
    }

    void RotateBase()
    {
        Vector3 directionToTarget = _targetPosition - _turretBase.position;
        Vector3 flatDirection = Vector3.ProjectOnPlane(directionToTarget, _turretBase.up);

        if (flatDirection.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(flatDirection, _turretBase.up);
        _turretBase.rotation = Quaternion.RotateTowards(
            _turretBase.rotation,
            targetRotation,
            _rotateSpeed * Time.deltaTime);
    }

    void RotateCannon()
    {
        if (_turretCannon == null) return;
        Vector3 directionToTarget = (_targetPosition - _turretCannon.position).normalized;
        Quaternion worldTargetRotation = Quaternion.LookRotation(directionToTarget);
        Quaternion localTargetRotation = Quaternion.Inverse(_turretCannon.parent.rotation)
            * worldTargetRotation;
        Vector3 euler = localTargetRotation.eulerAngles;
        float xAngle = euler.x;
        if (xAngle > 180f) xAngle -= 360f;
        xAngle = Mathf.Clamp(xAngle, -180f, 5f);
        Quaternion targetRot = Quaternion.Euler(xAngle, 0f, 0f);
        _turretCannon.localRotation = Quaternion.Slerp(
            _turretCannon.localRotation,
            targetRot,
            _rotateSpeed * Time.deltaTime);
    }

    [SerializeField] private LayerMask _playerLayerMask;

    void FireLaser()
    {
        if (_turretCannon == null) return;

        if (Physics.Raycast(_turretCannon.position,
            _turretCannon.forward, out var hit, _laserRange, _playerLayerMask))
        {
            if (_laserBeam != null)
            {
                float distance = Vector3.Distance(_turretCannon.position, hit.point);
                Vector3 beamScale = _laserBeam.transform.localScale;
                beamScale.z = distance;
                _laserBeam.transform.localScale = beamScale;
            }

            if (hit.collider.TryGetComponent<ISpaceDamageable>(out var damageable))
                damageable.TakeDamage(
                    Mathf.RoundToInt(_laserDamagePerTick * Time.deltaTime),
                    hit.point);
        }
        else
        {
            if (_laserBeam != null)
            {
                Vector3 beamScale = _laserBeam.transform.localScale;
                beamScale.z = _laserRange;
                _laserBeam.transform.localScale = beamScale;
            }
        }
    }
}