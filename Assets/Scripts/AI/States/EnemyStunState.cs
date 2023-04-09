using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunState : EnemyBaseState
{
    public EnemyStunState(SimpleEnemy context, EnemyStateFactory factory) : base(context, factory) { }

    public override void OnEnter(SimpleEnemy context)
    {
        context.StunAnimGO.SetActive(true);
        context.Movement = SimpleEnemy.MovementState.Stop;
        SoundManager.Instance.Play("Confused");
    }

    public override void OnUpdate(SimpleEnemy context)
    {
        context.StunTime -= Time.fixedDeltaTime;

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(SimpleEnemy context)
    {
        if (context.StunTime <= 0f)
        {
            SwitchState(Factory.Patrol());
        }
    }

    public override void OnExit(SimpleEnemy context)
    {
        context.gameObject.layer = 7;
        context.StunAnimGO.SetActive(false);
        //Debug.Log("Exit Stun");
    }

    public override void InitializeSubState(SimpleEnemy context)
    {
        
    }
}
