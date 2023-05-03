using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SturdyStunState : SturdyBaseState
{
    public SturdyStunState(Sturdy context, SturdyStateFactory factory) : base(context, factory) { }

    public override void OnEnter(Sturdy context)
    {
        context.StunAnimGO.SetActive(true);
        context.Movement = AIBase.MovementState.Stop;
        SoundManager.Instance.Play("Confused");
    }

    public override void OnUpdate(Sturdy context)
    {
        context.StunTime -= Time.fixedDeltaTime;

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(Sturdy context)
    {
        if (context.StunTime <= 0f)
        {
            if (context.CanISeeMyTarget)
                SwitchState(Factory.Chase());

            SwitchState(Factory.Patrol());
        }
    }

    public override void OnExit(Sturdy context)
    {
        context.MyUnit.gameObject.layer = 7;
        context.StunAnimGO.SetActive(false);
        //Debug.Log("Exit Stun");
    }

    public override void InitializeSubState(Sturdy context)
    {

    }
}
