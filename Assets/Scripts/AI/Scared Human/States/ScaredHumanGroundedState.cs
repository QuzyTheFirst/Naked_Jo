using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredHumanGroundedState : ScaredHumanBaseState
{
    public ScaredHumanGroundedState(ScaredHuman context, ScaredHumanStateFactory factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(ScaredHuman context)
    {
        Debug.Log("Grounded State Enter");

        InitializeSubState(context);
    }

    public override void OnUpdate(ScaredHuman context)
    {
        float currentSpeed = context.MyRigidbody.velocity.x;

        float targetSpeed = context.MovementDirection * context.MovementSpeed;
        targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, context.LerpAmount);

        float speedDif = targetSpeed - currentSpeed;

        float accelRate = targetSpeed == 0 ? context.Deceleration : context.Acceleration;

        context.MyRigidbody.velocity = new Vector2(context.MyRigidbody.velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / context.MyRigidbody.mass, context.MyRigidbody.velocity.y);

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(ScaredHuman context)
    {
        if (!context.IsGrounded)
        {
            SwitchState(Factory.Falling());
            return;
        }
    }

    public override void InitializeSubState(ScaredHuman context)
    {
        if (context.StunTime > 0f)
        {
            SetSubState(Factory.Stun());
            return;
        }

        SetSubState(Factory.Idle());
    }

    public override void OnExit(ScaredHuman context)
    {
        Debug.Log("Ground state exit");
    }
}
