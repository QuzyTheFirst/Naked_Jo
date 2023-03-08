using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState (SimpleEnemy context, EnemyStateFactory factory) : base(context, factory) { }

    public override void OnEnter(SimpleEnemy context)
    {
        context.Flip.TryToFlip(context.MovementDirection);

        context.MovementSpeed = context.RunSpeed;
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        //Debug.Log($"Chase State: {Time.time}");

        Vector2 dir = context.TargetUnit.position - context.transform.position;
        float movementDir = Mathf.Sign(dir.x);

        if(movementDir == 1)
            context.Movement = SimpleEnemy.MovementState.Right;
        else if (movementDir == -1)
            context.Movement = SimpleEnemy.MovementState.Left;

        context.Flip.TryToFlip(movementDir);

        RaycastHit2D floorHit = Physics2D.Raycast(context.transform.position, Vector2.down + Vector2.right * movementDir, 1f, context.GroundMask);

        if (floorHit.collider == false)
        {
            context.Rig.velocity *= Vector2.up;
        }

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if (context.StanTime > 0f)
        {
            SwitchState(Factory.Stun());
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnit.position);
        if(distance < context.AttackRadius)
        {
            SwitchState(Factory.Attack());
        }

        if (!context.CanISeeMyTarget())
        {
            SwitchState(Factory.Patrol());
        }
    }

    public override void OnExit(SimpleEnemy context)
    {
        
    }

    public override void InitializeSubState(SimpleEnemy context)
    {
        
    }
}
