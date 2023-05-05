using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleKatanaMan : AIBase
{
    [Header("LittleKatanMan")]
    [Header("Chase State")]
    [SerializeField] private float _chasePlayerAfterDissapearanceTime = 5f;
    private float _chasePlayerAfterDissapearanceTimer;

    [Header("Bullet deflection")]
    [SerializeField] private float _checkForBulletsRadius;
    [SerializeField] private float _bulletSpreadAmount =.5f;
    [SerializeField] private float _bulletDeflectionDuration = 1f;
    private float _bulletDeflectionTimer;
    private Collider2D[] _bulletsToDeflect;

    // Simple Enemy States
    private LittleKatanaManBaseState _currentState;
    private LittleKatanaManStateFactory _states;

    // Shoot
    private float _timeToNextShoot;

    public float TimeToNextShoot { get { return _timeToNextShoot; } set { _timeToNextShoot = value; } }

    public float ShootEvery { get { return MyWeaponController.GetWeaponParams().EnemyAttackRate; } }

    //States
    public LittleKatanaManBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    //Chase State
    public float ChasePlayerAfterDissapearanceTime { get { return _chasePlayerAfterDissapearanceTime; } }
    public float ChasePlayerAfterDissapearanceTimer { get { return _chasePlayerAfterDissapearanceTimer; } set { _chasePlayerAfterDissapearanceTimer = value; } }

    //Bullet Deflect State
    public Collider2D[] BulletsToDeflect { get { return _bulletsToDeflect; } set { _bulletsToDeflect = value; } }
    public float BulletSpreadAmount { get { return _bulletSpreadAmount; } }

    public float BulletDeflectionDuration { get { return _bulletDeflectionDuration; } }
    public float BulletDeflectionTimer { get { return _bulletDeflectionTimer; } set { _bulletDeflectionTimer = value; } }

    private void Start()
    {
        _states = new LittleKatanaManStateFactory(this);
        _currentState = _states.Grounded();

        _currentState.OnEnter(this);
    }

    protected override void FixedUpdate()
    {
        if (IsPossessed)
            return;

        base.FixedUpdate();

        if (CanISeeMyTarget)
        {
            _chasePlayerAfterDissapearanceTimer = _chasePlayerAfterDissapearanceTime;
        }
        else
        {
            _chasePlayerAfterDissapearanceTimer -= Time.fixedDeltaTime;
        }

        _bulletsToDeflect = CheckForBulletsAround();
        if(_bulletsToDeflect.Length > 0)
        {
            _bulletDeflectionTimer = _bulletDeflectionDuration;
        }

        _currentState.UpdateStates(this);
        Debug.Log($"Current State: {_currentState} | Current Sub State: {_currentState.GetSubState()}");
    }


    public override void Possess(LayerMask attackMask)
    {
        base.Possess(attackMask);

        AttackMask = attackMask;
        MyWeaponController.SetAttackMask(attackMask);

        _chasePlayerAfterDissapearanceTimer = 0f;
    }

    public override void UnPossess(LayerMask attackMask, Transform targetUnit)
    {
        base.UnPossess(attackMask, targetUnit);

        AttackMask = attackMask;
        MyWeaponController.SetAttackMask(attackMask);
    }

    public override void Stun(float time)
    {
        base.Stun(time);

        _currentState.UpdateStates(this);
    }

    public Collider2D[] CheckForBulletsAround()
    {
        Collider2D[] coll = Physics2D.OverlapCircleAll(transform.position, _checkForBulletsRadius, LayerMask.GetMask("Bullet"));
        if (coll != null)
        {
            return coll;
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _checkForBulletsRadius);
    }
}
