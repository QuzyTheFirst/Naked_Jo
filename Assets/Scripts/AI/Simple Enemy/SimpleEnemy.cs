using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : AIBase
{
    [Header("Simple Enemy")]

    [SerializeField] private bool _startWithIdle = false;

    [Header("Chase State")]
    [SerializeField] private float _chasePlayerAfterDissapearanceTime = 5f;
    private float _chasePlayerAfterDissapearanceTimer;

    // Simple Enemy States
    private SimpleEnemyBaseState _currentState;
    private SimpleEnemyStateFactory _states;

    // Shoot
    private float _timeToNextShoot;

    public bool StartWithIdle { get { return _startWithIdle;} set { _startWithIdle = value; } }

    public float TimeToNextShoot { get { return _timeToNextShoot; } set { _timeToNextShoot = value; } }
 
    public float ShootEvery { get { return MyWeaponController.GetWeaponParams().EnemyAttackRate; } }

    //States
    public SimpleEnemyBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    //Chase State
    public float ChasePlayerAfterDissapearanceTime { get { return _chasePlayerAfterDissapearanceTime; } }
    public float ChasePlayerAfterDissapearanceTimer { get { return _chasePlayerAfterDissapearanceTimer; } set { _chasePlayerAfterDissapearanceTimer = value; } }
  
    private void Start()
    {
        _states = new SimpleEnemyStateFactory(this);
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

        SearchForNewWeapon();

        _currentState.UpdateStates(this);
        //Debug.Log($"Current State: {_currentState} | Current Sub State: {_currentState.GetSubState()}");
        float distance = Vector2.Distance(transform.position, TargetUnitTf.position);
        //Debug.Log($"Target Unit: {TargetUnit} | Is in Attack radius: {distance < AttackRadius} | Can I see my target: {CanISeeMyTarget}");
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
}

