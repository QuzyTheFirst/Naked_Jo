using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredHumanRetreatState : ScaredHumanBaseState
{
    public ScaredHumanRetreatState(ScaredHuman context, ScaredHumanStateFactory factory) : base(context, factory) { }

    public override void OnEnter(ScaredHuman context)
    {
        context.MyFlip.TryToFlip(-context.MovementDirection);

        context.MovementSpeed = context.WalkSpeed;

        context.Movement = AIBase.MovementState.Stop;
    }

    public override void OnUpdate(ScaredHuman context)
    {
        if (context.StunTime > 0f || context.TargetUnit == null)
        {
            CheckSwitchStates(context);
            return;
        }

        Vector2 vectorToPlayer = context.LastPointWhereTargetWereSeen - (Vector2)context.transform.position;
        float playerDir = Mathf.Sign(vectorToPlayer.x);

        context.MyFlip.TryToFlip(playerDir);

        if (playerDir == 1)
        {
            context.Movement = AIBase.MovementState.Left;
        }
        else if (playerDir == -1)
        {
            context.Movement = AIBase.MovementState.Right;
        }

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(ScaredHuman context)
    {
        if (!context.CanISeeMyTarget)
        {
            Debug.Log("GoToIdle!");
            SwitchState(Factory.Idle());
        }
    }

    public override void OnExit(ScaredHuman context)
    {
        Debug.Log("Retreat state exit");
    }

    public override void InitializeSubState(ScaredHuman context)
    {

    }

}
