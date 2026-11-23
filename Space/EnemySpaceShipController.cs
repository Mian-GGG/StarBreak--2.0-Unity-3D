using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemySpaceShipController : SpaceShipController
{
    [SerializeField] float _patrolRange = 2000f, _attackRange = 1000f;
    [SerializeField] LayerMask _targetMask, _playerMask;

    enum ShipCombatState
    {
        None,
        Patrol,
        Attack,
        Reposition,
        Retreat
    }

    EnemyMovementBrain _movementBrain;
    EnemyWeaponBrain _weaponBrain;
    ShipCombatState _state = ShipCombatState.None;
    Transform _transform;

    GameObject _playerShip;

    GameObject PlayerShip
    {
        get
        {
            if (_playerShip == null)
                _playerShip = GameObject.FindGameObjectWithTag("Player");
            return _playerShip;
        }
    }
    Transform _target;

    public UnityEvent<int> ShipDestroyed = new UnityEvent<int>();

    #region Debug Info
    public string CombatState => _state.ToString();
    public string TargetName => _target ? _target.name : "none";
    public string DistanceToTarget => _target ?
        $"{Vector3.Distance(_target.position, _transform.position):F2}" : "";
    public string HealthLevel => $"{_healthHandler.Health}/{_healthHandler.MaxHealth}";
    #endregion

 
    bool InAttackRange => Vector3.Distance(
        PlayerShip.transform.position, _transform.position) <= _attackRange;
    bool ShouldRetreat => _healthHandler.Health < (_healthHandler.MaxHealth * 0.33f);
    bool ReachedPatrolTarget => Vector3.Distance(
        _target.position, _transform.position) < 0.15f;
    bool ShouldReposition => Physics.SphereCast(
        _transform.position, 3f, _transform.forward, out var hit, 100f, _playerMask);

    public override void OnEnable()
    {
        _transform = transform;
        _movementBrain = (EnemyMovementBrain)_movementControls;
        _weaponBrain = (EnemyWeaponBrain)_weaponControls;

        var player = GameObject.FindGameObjectWithTag("Player");
        //Debug.Log($"Player found: {player}");


        SetCombatState(ShipCombatState.Patrol);
        base.OnEnable();
    }

    void OnDisable()
    {
        ShipDestroyed.Invoke(GetInstanceID());
    }

    public override void Update()
    {
        SetCombatState(GetNextState());
        base.Update();
    }
    ShipCombatState GetNextState()
    {
        return _state switch
        {
            ShipCombatState.Patrol => HandlePatrol(),
            ShipCombatState.Attack => HandleAttack(),
            ShipCombatState.Reposition => HandleReposition(),
            ShipCombatState.Retreat => HandleRetreat(),
            _ => ShipCombatState.None
        };
    }

    ShipCombatState HandlePatrol()
    {
        if (ShouldRetreat) return ShipCombatState.Retreat;
        if (InAttackRange) return ShipCombatState.Attack;
        if (ReachedPatrolTarget)
            _target.position = Random.insideUnitSphere * _patrolRange;
        return ShipCombatState.Patrol;
    }

    ShipCombatState HandleAttack()
    {
        if (ShouldRetreat) return ShipCombatState.Retreat;
        return ShouldReposition ? ShipCombatState.Reposition : ShipCombatState.Attack;
    }



    ShipCombatState HandleReposition()
    {
        if (ShouldRetreat) return ShipCombatState.Retreat;
        return Vector3.Distance(_target.position, _transform.position) < 100f
            ? ShipCombatState.Attack
            : ShipCombatState.Reposition;
    }



    ShipCombatState HandleRetreat()
    {
        return ShipCombatState.Retreat;
    }

    void SetCombatState(ShipCombatState state)
    {
        if (_state == state) return;
        _state = state;
        switch (state)
        {
            case ShipCombatState.Patrol:
                if (!_target)
                {
                    _target = new GameObject("PatrolTarget").transform;
                    _target.position = Random.insideUnitSphere * _patrolRange;
                    _movementBrain.SetTarget(_target);
                }
                break;

            case ShipCombatState.Attack:
                if (_target) Destroy(_target.gameObject);
                _target = PlayerShip.transform;
                _movementBrain.SetTarget(_target);
                AssignWeaponTarget(_target, _attackRange, _targetMask);
                break;

            case ShipCombatState.Reposition:
                _movementBrain.SetTarget(_target = GetRepositionTarget());
                AssignWeaponTarget(null, 0, 0);
                break;

            case ShipCombatState.Retreat:
                _movementBrain.SetTarget(_target = GetRetreatTarget());
                AssignWeaponTarget(null, 0, 0);
                break;
        }
    }

    Transform GetRetreatTarget()
    {
        var direction = PlayerShip.transform.position - _transform.position;
        var target = new GameObject("RetreatTarget").transform;
        target.position = direction * -700f;
        return target;
    }

    Transform GetRepositionTarget()
    {
        var target = new GameObject("RepositionTarget").transform;
        var rand = Random.Range(1, 4);
        var direction = rand switch
        {
            1 => _transform.right,
            2 => _transform.right * -1,
            3 => _transform.up,
            _ => _transform.up * -1
        };
        target.position = _transform.position + direction * 250f;
        return target;
    }

    void AssignWeaponTarget(Transform target, float attackRange, int targetMask)
    {
        _weaponBrain.SetTarget(target, attackRange, targetMask);
    }
}