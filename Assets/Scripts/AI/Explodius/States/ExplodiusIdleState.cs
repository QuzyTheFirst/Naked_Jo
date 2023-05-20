using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodiusIdleState : ExplodiusBaseState
{
    public ExplodiusIdleState(Explodius context, ExplodiusStateFactory factory) : base(context, factory) { }

    public override void OnEnter(Explodius context)
    {
        context.Movement = AIBase.MovementState.Stop;

        context.TimerBeforeAction = context.TimeBeforeAction;

        SetWeaponTargetPos(context);
    }

    public override void OnUpdate(Explodius context)
    {
        if (context.StunTime > 0f)
        {
            CheckSwitchStates(context);
            return;
        }

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(Explodius context)
    {
        if (context.StunTime > 0f)
        {
            SwitchState(Factory.Explode());
            context.StartWithIdle = false;
            return;
        }

        if (context.TargetUnit == null)
        {
            return;
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if (context.CanISeeMyTarget && distance < context.AttackRadius)
        {
            SwitchState(Factory.Explode());
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

    private void SetWeaponTargetPos(Explodius context)
    {
        context.MyWeaponController.TargetPos = (Vector2)context.transform.position + Vector2.right;
    }

    public override void OnExit(Explodius context)
    {

    }

    public override void InitializeSubState(Explodius context)
    {

    }
}
