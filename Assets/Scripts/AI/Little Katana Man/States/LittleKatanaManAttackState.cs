using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleKatanaManAttackState : LittleKatanaManBaseState
{
    private Vector2 _attackPos;

    private bool _aimAtPlayer;

    public LittleKatanaManAttackState(LittleKatanaMan context, LittleKatanaManStateFactory factory) : base(context, factory) { }

    public override void OnEnter(LittleKatanaMan context)
    {
        context.Movement = AIBase.MovementState.Stop;
    }

    public override void OnUpdate(LittleKatanaMan context)
    {
        if (context.StunTime > 0f || context.TargetUnit == null || !context.CanISeeMyTarget)
        {
            CheckSwitchStates(context);
            return;
        }

        if (!context.MyWeaponController.IsAttacking)
        {
            _aimAtPlayer = true;
        }

        if (context.MyWeaponController.IsAttacking && _aimAtPlayer && context.TargetUnit != null)
        {
            if (context.TargetUnit.Player.IsRolling)
            {
                _aimAtPlayer = false;
            }
        }

        if (!context.MyWeaponController.IsAttacking && context.TargetUnit != null)
        {
            Vector2 dir = context.TargetUnitTf.position - context.transform.position;
            context.MyFlip.TryToFlip(Mathf.Sign(dir.x));
        }

        context.TimerBeforeAction -= Time.fixedDeltaTime;
        if (context.TimerBeforeAction > 0f)
            return;

        if (Time.time > context.TimeToNextShoot && context.TargetUnit != null)
        {
            // Here is my text bitch
            context.MyWeaponController.AIShoot(context.TargetUnit);

            if (context.MyWeaponController.IsAttacking)
            {
                _attackPos = context.TargetUnitTf.position;
            }

            context.MyWeaponController.ResetAmmo();

            context.TimeToNextShoot = Time.time + context.ShootEvery;
        }

        UpdateWeaponTargetPos(context);

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(LittleKatanaMan context)
    {
        if (context.StunTime > 0f)
        {
            SwitchState(Factory.Stun());
            return;
        }

        if (context.MyWeaponController.IsAttacking)
            return;

        if (context.BulletsToDeflect.Length > 0)
        {
            SwitchState(Factory.BulletsDeflect());
            return;
        }

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

    private void UpdateWeaponTargetPos(LittleKatanaMan context)
    {
        if (context.TargetUnit == null)
        {
            Debug.LogWarning("Target Unit hasn't been setted up!");
            return;
        }

        if (_aimAtPlayer && !context.MyWeaponController.IsAttacking)
            context.MyWeaponController.TargetPos = context.TargetUnitTf.position;
        else
            context.MyWeaponController.TargetPos = _attackPos;
    }

    public override void OnExit(LittleKatanaMan context)
    {

    }

    public override void InitializeSubState(LittleKatanaMan context)
    {

    }
}