using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyAttackState : EnemyBaseState
{
    private float _timer = 0;

    private bool _isAttacking = false;
    private float _attackTime;

    private Vector2 _attackPos;
    private Transform _attackTf;

    private bool _aimAtPlayer;

    public SimpleEnemyAttackState(SimpleEnemy context, SimpleEnemyStateFactory factory) : base(context, factory) { }

    public override void OnEnter(SimpleEnemy context)
    {
        context.Movement = SimpleEnemy.MovementState.Stop;
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        if(context.StunTime > 0f || context.TargetUnit == null || !context.CanISeeMyTarget)
        {
            CheckSwitchStates(context);
            return;
        }

        if (_attackTime > 0)
        {
            _attackTime -= Time.fixedDeltaTime;
        }
        else
        {
            _isAttacking = false;
            _aimAtPlayer = true;
        }

        if (_isAttacking && _aimAtPlayer && context.TargetUnit != null)
        {
            if (context.TargetUnit.Player.IsRolling)
            {
                _aimAtPlayer = false;
            }
        }

        if (!_isAttacking && context.TargetUnit != null)
        {
            Vector2 dir = context.TargetUnitTf.position - context.transform.position;
            context.Flip.TryToFlip(Mathf.Sign(dir.x));
        }

        context.TimerBeforeAction -= Time.fixedDeltaTime;
        if (context.TimerBeforeAction > 0f)
            return;

        if (Time.time > context.TimeToNextShoot && context.TargetUnit != null)
        {
            // Here is my text bitch
            _isAttacking = context.WeaponController.AIShoot(context.TargetUnit);

            if (_isAttacking)
            {
                _attackTime = context.WeaponController.FullAttackTime;
                _attackPos = context.TargetUnitTf.position;
                _attackTf = context.TargetUnitTf;
            }

            context.WeaponController.ResetAmmo();

            context.TimeToNextShoot = Time.time + context.ShootEvery;
        }

        UpdateWeaponTargetPos(context);

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if (context.StunTime > 0f)
        {
            SwitchState(Factory.Stun());
            return;
        }

        if (_isAttacking)
            return;

        if (context.TargetUnit == null)
        {
            SwitchState(Factory.Patrol());
            return;
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if (distance > context.AttackRadius || !context.CanISeeMyTarget)
        {
            SwitchState(Factory.Chase());
            return;
        }
    }

    private void UpdateWeaponTargetPos(SimpleEnemy context)
    {
        if (context.TargetUnit == null)
        {
            Debug.LogWarning("Target Unit hasn't been setted up!");
            return;
        }

        if (_aimAtPlayer && !_isAttacking)
            context.WeaponController.TargetPos = context.TargetUnitTf.position;
        else
            context.WeaponController.TargetPos = _attackPos;
    }

    public override void OnExit(SimpleEnemy context)
    {
        _timer = 0f;
    }
    
    public override void InitializeSubState(SimpleEnemy context)
    {
        
    }
}
