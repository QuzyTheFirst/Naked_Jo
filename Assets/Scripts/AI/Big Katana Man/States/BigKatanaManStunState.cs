using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigKatanaManStunState : BigKatanaManBaseState
{
    public BigKatanaManStunState(BigKatanaMan context, BigKatanaManStateFactory factory) : base(context, factory) { }

    public override void OnEnter(BigKatanaMan context)
    {
        context.StunAnimGO.SetActive(true);
        context.Movement = AIBase.MovementState.Stop;
        SoundManager.Instance.Play("Confused");
    }

    public override void OnUpdate(BigKatanaMan context)
    {
        context.StunTime -= Time.fixedDeltaTime;

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(BigKatanaMan context)
    {
        if (context.StunTime <= 0f)
        {
            if (context.CanISeeMyTarget)
                SwitchState(Factory.Chase());

            SwitchState(Factory.Patrol());
        }
    }

    public override void OnExit(BigKatanaMan context)
    {
        context.MyUnit.gameObject.layer = 7;
        context.StunAnimGO.SetActive(false);
        //Debug.Log("Exit Stun");
    }

    public override void InitializeSubState(BigKatanaMan context)
    {

    }
}
