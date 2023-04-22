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
        if (context.StunTime > 0f)
        {
            CheckSwitchStates(context);
            return;
        }

        //Debug.Log($"Chase State: {Time.time}");
        context.TimerBeforeAction -= Time.fixedDeltaTime;
        if (context.TimerBeforeAction > 0f)
            return;

        Vector2 dir = context.TargetUnitTf.position - context.transform.position;
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

        UpdateWeaponTargetPos(context);

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if (context.StunTime > 0f)
        {
            SwitchState(Factory.Stun());
            return;
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if(distance < context.AttackRadius)
        {
            SwitchState(Factory.Attack());
            return;
        }

        if (!context.CanISeeMyTarget())
        {
            SwitchState(Factory.Patrol());
            return;
        }
    }

    private void UpdateWeaponTargetPos(SimpleEnemy context)
    {
        if (context.TargetUnit == null)
        {
            Debug.LogWarning("Target Unit hasn't been setted up!");
            return;
        }

        context.WeaponController.TargetPos = context.TargetUnitTf.position;
    }

    public override void OnExit(SimpleEnemy context)
    {
        
    }

    public override void InitializeSubState(SimpleEnemy context)
    {
        
    }
}
