using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpingState : EnemyBaseState
{
    private float _jumpPower;

    public EnemyJumpingState(SimpleEnemy context, EnemyStateFactory factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(SimpleEnemy context)
    {
        InitializeSubState(context);

        if (context.DoJump)
        {
            JumpAction(context);
        }
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        if (context.JumpedOnHisOwn)
        {
            float currentSpeed = context.RigidBody.velocity.x;

            float targetSpeed = context.MovementDirection * context.MovementSpeed;
            targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, context.LerpAmount);

            float speedDif = targetSpeed - currentSpeed;

            float accelRate = targetSpeed == 0 ? context.Deceleration : context.Acceleration;

            context.RigidBody.velocity = new Vector2(context.RigidBody.velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / context.RigidBody.mass, context.RigidBody.velocity.y);
        }

        if (context.RigidBody.velocity.y > 0)
        {
            context.RigidBody.gravityScale = context.UnitController.UpwardMovementMultiplier;
        }
        else if (context.RigidBody.velocity.y == 0)
        {
            context.RigidBody.gravityScale = context.UnitController.DefaultGravityScale;
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

        if (!context.IsGrounded && context.RigidBody.velocity.y < 0f)
        {
            SwitchState(Factory.Falling());
            return;
        }
    }

    public override void InitializeSubState(SimpleEnemy context)
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

    public override void OnExit(SimpleEnemy context)
    {
        context.RigidBody.gravityScale = context.UnitController.DefaultGravityScale;

        context.JumpedOnHisOwn = false;
    }

    // Methods
    private void JumpAction(SimpleEnemy context)
    {
        _jumpPower = Mathf.Sqrt(-2f * Physics.gravity.y * context.UnitController.JumpHeight);

        //player.GroundRemember = 0f;
        //player.JumpPressedRemember = 0f;
        //player.StepsSinceLastJump = 0;

        if (context.RigidBody.velocity.y > 0f)
        {
            _jumpPower = Mathf.Max(_jumpPower - context.RigidBody.velocity.y, 0f);
        }

        context.RigidBody.velocity = Vector2.up * _jumpPower + Vector2.right * context.RigidBody.velocity.x;

        context.DoJump = false;
    }
}
