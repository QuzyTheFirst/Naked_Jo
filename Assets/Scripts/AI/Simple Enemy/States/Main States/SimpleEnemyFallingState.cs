using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyFallingState : SimpleEnemyBaseState
{
    public SimpleEnemyFallingState(SimpleEnemy context, SimpleEnemyStateFactory factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(SimpleEnemy context)
    {
        InitializeSubState(context);
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        if (context.FallenOnHisOwn)
        {
            float currentSpeed = context.MyRigidbody.velocity.x;

            float targetSpeed = context.MovementDirection * context.MovementSpeed;
            targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, context.LerpAmount);

            float speedDif = targetSpeed - currentSpeed;

            float accelRate = targetSpeed == 0 ? context.Deceleration : context.Acceleration;

            context.MyRigidbody.velocity = new Vector2(context.MyRigidbody.velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / context.MyRigidbody.mass, context.MyRigidbody.velocity.y);
        }

        if (context.MyRigidbody.velocity.y < 0)
        {
            context.MyRigidbody.gravityScale = context.MyPlayerController.DownwardMovementMultiplier;
        }
        else if (context.MyRigidbody.velocity.y == 0)
        {
            context.MyRigidbody.gravityScale = context.MyPlayerController.DefaultGravityScale;
        }

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if (context.IsGrounded)
        {
            SwitchState(Factory.Grounded());
            return;
        }

        if(!context.IsGrounded && context.MyRigidbody.velocity.y > 0f)
        {
            SwitchState(Factory.Jumping());
            return;
        }
    }

    public override void InitializeSubState(SimpleEnemy context)
    {
        if (context.StunTime > 0f)
        {
            SetSubState(Factory.Stun());
            return;
        }

        if (context.FallenOnHisOwn)
        {
            if (context.TargetUnit == null)
                return;

            SetSubState(Factory.Chase());
        }
    }

    public override void OnExit(SimpleEnemy context)
    {
        context.MyRigidbody.gravityScale = context.MyPlayerController.DefaultGravityScale;

        context.FallenOnHisOwn = false;
    }
}
