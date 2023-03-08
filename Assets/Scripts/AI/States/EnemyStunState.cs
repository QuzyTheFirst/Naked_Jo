using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunState : EnemyBaseState
{
    public EnemyStunState(SimpleEnemy context, EnemyStateFactory factory) : base(context, factory) { }

    public override void OnEnter(SimpleEnemy context)
    {
        Debug.Log("Stunned");
        context.Movement = SimpleEnemy.MovementState.Stop;
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        context.StanTime -= Time.fixedDeltaTime;

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if (context.StanTime <= 0f)
        {
            SwitchState(Factory.Patrol());
        }
    }

    public override void OnExit(SimpleEnemy context)
    {
        context.gameObject.layer = 7;
        Debug.Log("Exit Stun");
    }

    public override void InitializeSubState(SimpleEnemy context)
    {
        
    }
}
