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

        context.Movement = SimpleEnemy.MovementState.Stop;
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        if (context.StunTime > 0f || context.TargetUnit == null)
        {
            CheckSwitchStates(context);
            return;
        }

        //Debug.Log($"Chase State: {Time.time}");
        context.TimerBeforeAction -= Time.fixedDeltaTime;
        if (context.TimerBeforeAction > 0f)
            return;

        KeepGoingOrStop(context);

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

        if (context.ChasePlayerAfterDissapearanceTimer <= 0f || context.TargetUnit == null)
        {
            SwitchState(Factory.Patrol());
            return;
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if(distance < context.AttackRadius && context.CanISeeMyTarget)
        {
            SwitchState(Factory.Attack());
            return;
        }
    }
    public override void InitializeSubState(SimpleEnemy context)
    {
        
    }
    public override void OnExit(SimpleEnemy context)
    {
        
    }

    //My Methods
    private void UpdateWeaponTargetPos(SimpleEnemy context)
    {
        if (context.TargetUnit == null)
        {
            Debug.LogWarning("Target Unit hasn't been setted up!");
            return;
        }

        context.WeaponController.TargetPos = context.LastPointWhereTargetWereSeen;
    }

    private void KeepGoingOrStop(SimpleEnemy context)
    {
        Vector2 dir = context.LastPointWhereTargetWereSeen - (Vector2)context.transform.position;
        float movementDir = Mathf.Sign(dir.x);

        context.Flip.TryToFlip(movementDir);

        bool floorCheck = FloorCheck(context, movementDir);

        bool wallCheck = WallCheck(context, movementDir);

        if (!floorCheck && !wallCheck)
        {
            if (movementDir == 1)
            {
                context.Movement = SimpleEnemy.MovementState.Right;
            }
            else if (movementDir == -1)
            {
                context.Movement = SimpleEnemy.MovementState.Left;
            }
        }
    }

    private bool WallCheck(SimpleEnemy context, float movementDir)
    {
        //Wall
        RaycastHit2D wallHit = Physics2D.Raycast(context.transform.position, Vector2.right * movementDir, 1f, context.GroundMask);
        RaycastHit2D upperGroundHit = Physics2D.Raycast(context.transform.position, Vector2.up + Vector2.right * movementDir, 1f, context.GroundMask);
        Debug.DrawLine(context.transform.position, context.transform.position + Vector3.up + Vector3.right * movementDir, Color.red);

        if (wallHit.collider == true && upperGroundHit.collider == true)
        {
            context.Movement = SimpleEnemy.MovementState.Stop;
            return true;
        }
        else if (wallHit.collider == true && upperGroundHit.collider == false)
        {
            if (movementDir == 1)
            {
                context.Movement = SimpleEnemy.MovementState.Right;
            }
            else if (movementDir == -1)
            {
                context.Movement = SimpleEnemy.MovementState.Left;
            }

            context.DoJump = true;
            context.JumpedOnHisOwn = true;
            context.FallenOnHisOwn = true;
            return true;
        }
        return false;
    }

    private static bool FloorCheck(SimpleEnemy context, float movementDir)
    {
        //Floor
        RaycastHit2D floorHit = Physics2D.Raycast(context.transform.position, Vector2.down + Vector2.right * movementDir, 1f, context.GroundMask);
        RaycastHit2D lowerGroundHit = Physics2D.Raycast(context.transform.position + Vector3.right * .6f * movementDir, Vector2.down, 2f, context.GroundMask);
        Debug.DrawLine(context.transform.position + Vector3.right * .6f * movementDir, context.transform.position + Vector3.right * .6f * movementDir + Vector3.down * 2f, Color.red);

        if (floorHit.collider == false && lowerGroundHit.collider == false)
        {
            context.RigidBody.velocity *= Vector2.up;
            context.Movement = SimpleEnemy.MovementState.Stop;
            return true;
        }
        else if (floorHit.collider == false && lowerGroundHit.collider == true)
        {
            if (movementDir == 1)
            {
                context.Movement = SimpleEnemy.MovementState.Right;
            }
            else if (movementDir == -1)
            {
                context.Movement = SimpleEnemy.MovementState.Left;
            }

            context.FallenOnHisOwn = true;
            return true;
        }
        return false;
    }
}
