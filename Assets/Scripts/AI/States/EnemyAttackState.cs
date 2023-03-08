using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    private float _timer = 0;

    public EnemyAttackState(SimpleEnemy context, EnemyStateFactory factory) : base(context, factory) { }

    public override void OnEnter(SimpleEnemy context)
    {
        context.Movement = SimpleEnemy.MovementState.Stop;
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        //Debug.Log($"Attack State: {Time.time}");

        Vector2 dir = context.TargetUnit.position - context.transform.position;
        context.Flip.TryToFlip(Mathf.Sign(dir.x));

        //_timer += Time.deltaTime;
        //if (_timer >= context.TimerBeforeAttack)
        //{
            if (Time.time > context.TimeToNextShoot)
            {
                context.WeaponController.Shoot();

                context.WeaponController.ResetAmmo();

                context.TimeToNextShoot = Time.time + context.ShootEvery;
            }
        //}
        
        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if (context.StanTime > 0f)
        {
            SwitchState(Factory.Stun());
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnit.position);
        if (distance > context.AttackRadius)
        {
            SwitchState(Factory.Chase());
        }

        if (!context.CanISeeMyTarget())
        {
            SwitchState(Factory.Patrol());
        }
    }

    public override void OnExit(SimpleEnemy context)
    {
        _timer = 0f;
    }
    
    public override void InitializeSubState(SimpleEnemy context)
    {
        
    }
}
