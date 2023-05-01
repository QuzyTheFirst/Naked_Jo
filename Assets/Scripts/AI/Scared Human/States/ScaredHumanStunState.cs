using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredHumanStunState : ScaredHumanBaseState
{
    public ScaredHumanStunState(ScaredHuman context, ScaredHumanStateFactory factory) : base(context, factory) { }

    public override void OnEnter(ScaredHuman context)
    {
        Debug.Log("Stun state enter");
        context.StunAnimGO.SetActive(true);
        context.Movement = AIBase.MovementState.Stop;
        SoundManager.Instance.Play("Confused");
    }

    public override void OnUpdate(ScaredHuman context)
    {
        context.StunTime -= Time.fixedDeltaTime;

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(ScaredHuman context)
    {
        if (context.StunTime <= 0f)
        {
            if (context.CanISeeMyTarget)
                SwitchState(Factory.Retreat());

            SwitchState(Factory.Idle());
        }
    }

    public override void OnExit(ScaredHuman context)
    {
        Debug.Log("Stun state exit");
        context.MyUnit.gameObject.layer = 7;
        context.StunAnimGO.SetActive(false);
        //Debug.Log("Exit Stun");
    }

    public override void InitializeSubState(ScaredHuman context)
    {

    }
}
