using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleKatanaManJumpingState : LittleKatanaManBaseState
{
    private float _jumpPower;

    public LittleKatanaManJumpingState(LittleKatanaMan context, LittleKatanaManStateFactory factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(LittleKatanaMan context)
    {
        InitializeSubState(context);

        if (context.DoJump)
        {
            JumpAction(context);
        }
    }

    public override void OnUpdate(LittleKatanaMan context)
    {
        if (context.JumpedOnHisOwn)
        {
            float currentSpeed = context.MyRigidbody.velocity.x;

            float targetSpeed = context.MovementDirection * context.MovementSpeed;
            targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, context.LerpAmount);

            float speedDif = targetSpeed - currentSpeed;

            float accelRate = targetSpeed == 0 ? context.Deceleration : context.Acceleration;

            context.MyRigidbody.velocity = new Vector2(context.MyRigidbody.velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / context.MyRigidbody.mass, context.MyRigidbody.velocity.y);
        }

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

    public override void CheckSwitchStates(LittleKatanaMan context)
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

    public override void InitializeSubState(LittleKatanaMan context)
    {
        if (context.StunTime > 0f)
        {
            SetSubState(Factory.Stun());
        }

        if (context.JumpedOnHisOwn)
        {
            if (context.TargetUnit == null)
                return;

            SetSubState(Factory.Chase());
            return;
        }
    }

    public override void OnExit(LittleKatanaMan context)
    {
        context.MyRigidbody.gravityScale = context.MyPlayerController.DefaultGravityScale;

        context.JumpedOnHisOwn = false;
    }

    // Methods
    private void JumpAction(LittleKatanaMan context)
    {
        _jumpPower = Mathf.Sqrt(-2f * Physics.gravity.y * context.MyPlayerController.JumpHeight);

        //player.GroundRemember = 0f;
        //player.JumpPressedRemember = 0f;
        //player.StepsSinceLastJump = 0;

        if (context.MyRigidbody.velocity.y > 0f)
        {
            _jumpPower = Mathf.Max(_jumpPower - context.MyRigidbody.velocity.y, 0f);
        }

        context.MyRigidbody.velocity = Vector2.up * _jumpPower + Vector2.right * context.MyRigidbody.velocity.x;

        context.DoJump = false;
    }
}
