using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNotGroundedState : EnemyBaseState
{
    public EnemyNotGroundedState(SimpleEnemy context, EnemyStateFactory factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(SimpleEnemy context)
    {
        InitializeSubState(context);
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if (context.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void InitializeSubState(SimpleEnemy context)
    {
        
    }

    public override void OnExit(SimpleEnemy context)
    {
        
    }
}
