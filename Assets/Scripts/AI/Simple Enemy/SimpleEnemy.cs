using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : Enemy
{
    [Header("Simple Enemy")]
    [Header("Chase State")]
    [SerializeField] private float _chasePlayerAfterDissapearanceTime = 5f;
    private float _chasePlayerAfterDissapearanceTimer;

    // Simple Enemy States
    private SimpleEnemyStateFactory _states;

    // Shoot
    private float _timeToNextShoot;

    public float TimeToNextShoot { get { return _timeToNextShoot; } set { _timeToNextShoot = value; } }
 
    public float ShootEvery { get { return WeaponController.GetWeaponParams().EnemyAttackRate; } }

    //Chase State
    public float ChasePlayerAfterDissapearanceTime { get { return _chasePlayerAfterDissapearanceTime; } }
    public float ChasePlayerAfterDissapearanceTimer { get { return _chasePlayerAfterDissapearanceTimer; } set { _chasePlayerAfterDissapearanceTimer = value; } }
  
    private void Start()
    {
        _states = new SimpleEnemyStateFactory(this);
        CurrentState = _states.Grounded();

        CurrentState.OnEnter(this);
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

        CurrentState.UpdateStates(this);
    } 


    public override void Possess(LayerMask attackMask)
    {
        base.Possess(attackMask);

        AttackMask = attackMask;
        _weaponController.SetAttackMask(attackMask);

        _chasePlayerAfterDissapearanceTimer = 0f;
    }

    public override void UnPossess(LayerMask attackMask, Transform targetUnit)
    {
        base.UnPossess(attackMask, targetUnit);

        AttackMask = attackMask;
        _weaponController.SetAttackMask(attackMask);
    }

    public override void Stun(float time)
    {
        base.Stun(time);

        CurrentState.UpdateStates(this);
    }
}

