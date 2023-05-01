using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredHumanJumpingState : ScaredHumanBaseState
{
    public ScaredHumanJumpingState(ScaredHuman context, ScaredHumanStateFactory factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(ScaredHuman context)
    {
        Debug.Log("Jumping state enter");
        InitializeSubState(context);
    }

    public override void OnUpdate(ScaredHuman context)
    {
        if (context.MyRigidbody.velocity.y > 0)
        {
            context.MyRigidbody.gravityScale = context.MyPlayerController.UpwardMovementMultiplier;
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

        if (!context.IsGrounded && context.MyRigidbody.velocity.y < 0f)
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
        }
    }

    public override void OnExit(ScaredHuman context)
    {
        Debug.Log("Jumping state exit");
        context.MyRigidbody.gravityScale = context.MyPlayerController.DefaultGravityScale;

        context.JumpedOnHisOwn = false;
    }
}
