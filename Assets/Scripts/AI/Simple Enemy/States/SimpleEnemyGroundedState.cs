using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyGroundedState : EnemyBaseState
{
    public SimpleEnemyGroundedState(SimpleEnemy context, SimpleEnemyStateFactory factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(SimpleEnemy context)
    {
        InitializeSubState(context);
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        float currentSpeed = context.RigidBody.velocity.x;

        float targetSpeed = context.MovementDirection * context.MovementSpeed;
        targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, context.LerpAmount);

        float speedDif = targetSpeed - currentSpeed;

        float accelRate = targetSpeed == 0 ? context.Deceleration : context.Acceleration;

        context.RigidBody.velocity = new Vector2(context.RigidBody.velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / context.RigidBody.mass, context.RigidBody.velocity.y);


        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
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

    public override void InitializeSubState(SimpleEnemy context)
    {
        if (context.StunTime > 0f)
        {
            SetSubState(Factory.Stun());
            return;
        }

        if (context.TargetUnit == null)
        {
            SetSubState(Factory.Patrol());
            return;
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if (context.CanISeeMyTarget && distance < context.AttackRadius)
        {
            SetSubState(Factory.Attack());
            return;
        }

        if (context.CanISeeMyTarget || context.ChasePlayerAfterDissapearanceTimer > 0f)
        {
            SetSubState(Factory.Chase());
            return;
        }

        SetSubState(Factory.Patrol());
    }

    public override void OnExit(SimpleEnemy context)
    {
        
    }
}
