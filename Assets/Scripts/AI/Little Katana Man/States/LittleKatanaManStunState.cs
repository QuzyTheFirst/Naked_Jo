using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleKatanaManStunState : LittleKatanaManBaseState
{
    public LittleKatanaManStunState(LittleKatanaMan context, LittleKatanaManStateFactory factory) : base(context, factory) { }

    public override void OnEnter(LittleKatanaMan context)
    {
        context.StunAnimGO.SetActive(true);
        context.Movement = AIBase.MovementState.Stop;
        SoundManager.Instance.Play("Confused");
    }

    public override void OnUpdate(LittleKatanaMan context)
    {
        context.StunTime -= Time.fixedDeltaTime;

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(LittleKatanaMan context)
    {
        if (context.StunTime <= 0f)
        {
            if (context.CanISeeMyTarget)
                SwitchState(Factory.Chase());

            SwitchState(Factory.Patrol());
        }
    }

    public override void OnExit(LittleKatanaMan context)
    {
        context.MyUnit.gameObject.layer = 7;
        context.StunAnimGO.SetActive(false);
        //Debug.Log("Exit Stun");
    }

    public override void InitializeSubState(LittleKatanaMan context)
    {

    }
}
