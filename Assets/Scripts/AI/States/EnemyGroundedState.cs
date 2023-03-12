using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundedState : EnemyBaseState
{
    public EnemyGroundedState(SimpleEnemy context, EnemyStateFactory factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(SimpleEnemy context)
    {
        InitializeSubState(context);
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        float currentSpeed = context.Rig.velocity.x;

        float targetSpeed = context.MovementDirection * context.MovementSpeed;
        targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, context.LerpAmount);

        float speedDif = targetSpeed - currentSpeed;

        float accelRate = targetSpeed == 0 ? context.Deceleration : context.Acceleration;

        context.Rig.velocity = new Vector2(context.Rig.velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / context.Rig.mass, context.Rig.velocity.y);


        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if (!context.IsGrounded)
        {
            SwitchState(Factory.NotGrounded());
        }
    }

    public override void InitializeSubState(SimpleEnemy context)
    {
        if (context.StunTime > 0f)
        {
            SwitchState(Factory.Stun());
        }
        else 
        { 
            SetSubState(Factory.Patrol());
        }
    }

    public override void OnExit(SimpleEnemy context)
    {
        
    }
}
