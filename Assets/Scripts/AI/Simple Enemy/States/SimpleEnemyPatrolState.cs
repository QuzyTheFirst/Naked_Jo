using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyPatrolState : EnemyBaseState
{
    public SimpleEnemyPatrolState(SimpleEnemy context, SimpleEnemyStateFactory factory) : base(context, factory) { }

    public override void OnEnter(SimpleEnemy context)
    {
        if (context.Movement == SimpleEnemy.MovementState.Stop)
            context.Movement = SimpleEnemy.MovementState.Right;

        context.MovementSpeed = context.WalkSpeed;

        context.Flip.TryToFlip(context.MovementDirection);

        context.TimerBeforeAction = context.TimeBeforeAction;
    }

    public override void OnUpdate(SimpleEnemy context)
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
            if (context.Movement == SimpleEnemy.MovementState.Right)
            {
                context.Flip.TryToFlip(-1);
                context.Movement = SimpleEnemy.MovementState.Left;
            }
            else
            {
                context.Flip.TryToFlip(1);
                context.Movement = SimpleEnemy.MovementState.Right;
            }
        }

        if (wallHit.collider == true)
        {
            if (context.Movement == SimpleEnemy.MovementState.Right)
            {
                context.Flip.TryToFlip(-1);
                context.Movement = SimpleEnemy.MovementState.Left;
            }
            else
            {
                context.Flip.TryToFlip(1);
                context.Movement = SimpleEnemy.MovementState.Right;
            }
        }

        UpdateWeaponTargetPos(context);

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if(context.StunTime > 0f)
        {
            SwitchState(Factory.Stun());
            return;
        }

        if (context.TargetUnit == null)
            return;

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if(context.CanISeeMyTarget &&  distance < context.AttackRadius)
        {
            SwitchState(Factory.Attack());
            return;
        }

        if (context.CanISeeMyTarget)
        {
            SwitchState(Factory.Chase());
            return;
        }
    }

    private void UpdateWeaponTargetPos(SimpleEnemy context)
    {
        context.WeaponController.TargetPos = (Vector2)context.transform.position + Vector2.right * context.MovementDirection;
    }

    public override void OnExit(SimpleEnemy context)
    {
        
    }

    public override void InitializeSubState(SimpleEnemy context)
    {
        
    }
}
