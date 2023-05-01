using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredHumanFallingState : ScaredHumanBaseState
{
    public ScaredHumanFallingState(ScaredHuman context, ScaredHumanStateFactory factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(ScaredHuman context)
    {
        Debug.Log("Falling State Enter");
        InitializeSubState(context);
    }

    public override void OnUpdate(ScaredHuman context)
    {
        if (context.MyRigidbody.velocity.y > 0)
        {
            context.MyRigidbody.gravityScale = context.MyPlayerController.UpwardMovementMultiplier;
        }
        else if (context.MyRigidbody.velocity.y < 0)
        {
            context.MyRigidbody.gravityScale = context.MyPlayerController.DownwardMovementMultiplier;
        }
        else if (context.MyRigidbody.velocity.y == 0)
        {
            context.MyRigidbody.gravityScale = context.MyPlayerController.DefaultGravityScale;
        }

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(ScaredHuman context)
    {
        if (context.IsGrounded)
        {
            SwitchState(Factory.Grounded());
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
    }

    public override void OnExit(ScaredHuman context)
    {
        Debug.Log("Falling State Exit");
        context.MyRigidbody.gravityScale = context.MyPlayerController.DefaultGravityScale;
    }
}
