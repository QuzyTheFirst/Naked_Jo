using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodiusChaseState : ExplodiusBaseState
{
    public ExplodiusChaseState(Explodius context, ExplodiusStateFactory factory) : base(context, factory) { }

    public override void OnEnter(Explodius context)
    {
        context.MyFlip.TryToFlip(context.MovementDirection);

        context.MovementSpeed = context.RunSpeed;

        context.Movement = AIBase.MovementState.Stop;
    }

    public override void OnUpdate(Explodius context)
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

    public override void CheckSwitchStates(Explodius context)
    {
        if (context.StunTime > 0f)
        {
            SwitchState(Factory.Explode());
            return;
        }

        if (context.ChasePlayerAfterDissapearanceTimer <= 0f || context.TargetUnit == null)
        {
            SwitchState(Factory.Patrol());
            return;
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if (distance < context.DistanceToStartExplosion && context.CanISeeMyTarget)
        {
            SwitchState(Factory.Explode());
            return;
        }
    }
    public override void InitializeSubState(Explodius context)
    {

    }
    public override void OnExit(Explodius context)
    {

    }

    //My Methods
    private void UpdateWeaponTargetPos(Explodius context)
    {
        if (context.TargetUnit == null)
        {
            Debug.LogWarning("Target Unit hasn't been setted up!");
            return;
        }

        context.MyWeaponController.TargetPos = context.LastPointWhereTargetWereSeen;
    }

    private void KeepGoingOrStop(Explodius context)
    {
        Vector2 dir = context.LastPointWhereTargetWereSeen - (Vector2)context.transform.position;
        float movementDir = Mathf.Sign(dir.x);

        context.MyFlip.TryToFlip(movementDir);

        bool floorCheck = FloorCheck(context, movementDir);

        bool wallCheck = WallCheck(context, movementDir);

        if (!floorCheck && !wallCheck)
        {
            if (movementDir == 1)
            {
                context.Movement = AIBase.MovementState.Right;
            }
            else if (movementDir == -1)
            {
                context.Movement = AIBase.MovementState.Left;
            }
        }
    }

    private bool WallCheck(Explodius context, float movementDir)
    {
        //Wall
        RaycastHit2D wallHit = Physics2D.Raycast(context.transform.position, Vector2.right * movementDir, 1f, context.GroundMask);
        RaycastHit2D upperGroundHit = Physics2D.Raycast(context.transform.position, Vector2.up + Vector2.right * movementDir, 1f, context.GroundMask);
        Debug.DrawLine(context.transform.position, context.transform.position + Vector3.up + Vector3.right * movementDir, Color.red);

        if (wallHit.collider == true && upperGroundHit.collider == true)
        {
            context.Movement = AIBase.MovementState.Stop;
            return true;
        }
        else if (wallHit.collider == true && upperGroundHit.collider == false)
        {
            if (movementDir == 1)
            {
                context.Movement = AIBase.MovementState.Right;
            }
            else if (movementDir == -1)
            {
                context.Movement = AIBase.MovementState.Left;
            }

            context.DoJump = true;
            context.JumpedOnHisOwn = true;
            context.FallenOnHisOwn = true;
            return true;
        }
        return false;
    }

    private static bool FloorCheck(Explodius context, float movementDir)
    {
        //Floor
        RaycastHit2D floorHit = Physics2D.Raycast(context.transform.position, Vector2.down + Vector2.right * movementDir, 1f, context.GroundMask);
        RaycastHit2D lowerGroundHit = Physics2D.Raycast(context.transform.position + Vector3.right * .6f * movementDir, Vector2.down, 2f, context.GroundMask);
        Debug.DrawLine(context.transform.position + Vector3.right * .6f * movementDir, context.transform.position + Vector3.right * .6f * movementDir + Vector3.down * 2f, Color.red);

        if (floorHit.collider == false && lowerGroundHit.collider == false)
        {
            context.MyRigidbody.velocity *= Vector2.up;
            context.Movement = AIBase.MovementState.Stop;
            return true;
        }
        else if (floorHit.collider == false && lowerGroundHit.collider == true)
        {
            if (movementDir == 1)
            {
                context.Movement = AIBase.MovementState.Right;
            }
            else if (movementDir == -1)
            {
                context.Movement = AIBase.MovementState.Left;
            }

            context.FallenOnHisOwn = true;
            return true;
        }
        return false;
    }
}
