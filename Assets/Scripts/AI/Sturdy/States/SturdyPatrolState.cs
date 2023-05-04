using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SturdyPatrolState : SturdyBaseState
{
    public SturdyPatrolState(Sturdy context, SturdyStateFactory factory) : base(context, factory) { }

    public override void OnEnter(Sturdy context)
    {
        if (context.Movement == AIBase.MovementState.Stop)
            context.Movement = AIBase.MovementState.Right;

        context.MovementSpeed = context.WalkSpeed;

        context.MyFlip.TryToFlip(context.MovementDirection);

        context.TimerBeforeAction = context.TimeBeforeAction;
    }

    public override void OnUpdate(Sturdy context)
    {
        if (context.StunTime > 0f)
        {
            CheckSwitchStates(context);
            return;
        }

        float movementDirection = context.MovementDirection;

        RaycastHit2D floorHit = Physics2D.Raycast(context.transform.position, Vector2.down + Vector2.right * movementDirection, 1f, context.GroundMask);
        RaycastHit2D wallHit = Physics2D.Raycast(context.transform.position, Vector2.right * movementDirection, 1f, context.GroundMask);

        if (floorHit.collider == false)
        {
            if (context.Movement == AIBase.MovementState.Right)
            {
                context.MyFlip.TryToFlip(-1);
                context.Movement = AIBase.MovementState.Left;
            }
            else
            {
                context.MyFlip.TryToFlip(1);
                context.Movement = AIBase.MovementState.Right;
            }
        }

        if (wallHit.collider == true)
        {
            if (context.Movement == AIBase.MovementState.Right)
            {
                context.MyFlip.TryToFlip(-1);
                context.Movement = AIBase.MovementState.Left;
            }
            else
            {
                context.MyFlip.TryToFlip(1);
                context.Movement = AIBase.MovementState.Right;
            }
        }

        UpdateWeaponTargetPos(context);

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(Sturdy context)
    {
        if (context.StunTime > 0f)
        {
            SwitchState(Factory.Stun());
            return;
        }

        if (context.TargetUnit == null)
            return;

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if (context.CanISeeMyTarget && distance < context.AttackRadius)
        {
            SwitchState(Factory.Attack());
            return;
        }

        if (context.DoRoll)
        {
            SwitchState(Factory.Dash());
            return;
        }

        if (context.CanISeeMyTarget)
        {
            SwitchState(Factory.Chase());
            return;
        }
    }

    private void UpdateWeaponTargetPos(Sturdy context)
    {
        context.MyWeaponController.TargetPos = (Vector2)context.transform.position + Vector2.right * context.MovementDirection;
    }

    public override void OnExit(Sturdy context)
    {

    }

    public override void InitializeSubState(Sturdy context)
    {

    }
}
