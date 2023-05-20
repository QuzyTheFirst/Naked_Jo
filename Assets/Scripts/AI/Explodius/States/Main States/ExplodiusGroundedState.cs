using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodiusGroundedState : ExplodiusBaseState
{
    public ExplodiusGroundedState(Explodius context, ExplodiusStateFactory factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(Explodius context)
    {
        InitializeSubState(context);
    }

    public override void OnUpdate(Explodius context)
    {
        float currentSpeed = context.MyRigidbody.velocity.x;

        float targetSpeed = context.MovementDirection * context.MovementSpeed;
        targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, context.LerpAmount);

        float speedDif = targetSpeed - currentSpeed;

        float accelRate = targetSpeed == 0 ? context.Deceleration : context.Acceleration;

        context.MyRigidbody.velocity = new Vector2(context.MyRigidbody.velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / context.MyRigidbody.mass, context.MyRigidbody.velocity.y);


        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(Explodius context)
    {
        if (context.DoJump)
        {
            SwitchState(Factory.Jumping());
            return;
        }

        if (!context.IsGrounded)
        {
            SwitchState(Factory.Falling());
        }
    }

    public override void InitializeSubState(Explodius context)
    {
        if (context.StunTime > 0f)
        {
            SetSubState(Factory.Explode());
            return;
        }

        if (context.StartWithIdle)
        {
            SetSubState(Factory.Idle());
            return;
        }

        if (context.TargetUnit == null)
        {
            SetSubState(Factory.Patrol());
            return;
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if (context.CanISeeMyTarget && distance < context.DistanceToStartExplosion)
        {
            SetSubState(Factory.Explode());
            return;
        }

        if (context.CanISeeMyTarget || context.ChasePlayerAfterDissapearanceTimer > 0f)
        {
            SetSubState(Factory.Chase());
            return;
        }

        SetSubState(Factory.Patrol());
    }

    public override void OnExit(Explodius context)
    {

    }
}
