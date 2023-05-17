using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyIdleState : SimpleEnemyBaseState
{
    public SimpleEnemyIdleState(SimpleEnemy context, SimpleEnemyStateFactory factory) : base(context, factory) { }

    public override void OnEnter(SimpleEnemy context)
    {
        context.Movement = AIBase.MovementState.Stop;

        context.TimerBeforeAction = context.TimeBeforeAction;

        SetWeaponTargetPos(context);
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        if (context.StunTime > 0f)
        {
            CheckSwitchStates(context);
            return;
        }

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if(context.StunTime > 0f)
        {
            SwitchState(Factory.Stun());
            context.StartWithIdle = false;
            return;
        }

        if (context.TargetUnit == null) {
            return;
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if(context.CanISeeMyTarget &&  distance < context.AttackRadius)
        {
            SwitchState(Factory.Attack());
            context.StartWithIdle = false;
            return;
        }

        if (context.CanISeeMyTarget)
        {
            SwitchState(Factory.Chase());
            context.StartWithIdle = false;
            return;
        }
    }

    private void SetWeaponTargetPos(SimpleEnemy context)
    {
        context.MyWeaponController.TargetPos = (Vector2)context.transform.position + Vector2.right;
    }

    public override void OnExit(SimpleEnemy context)
    {
        
    }

    public override void InitializeSubState(SimpleEnemy context)
    {
        
    }
}
