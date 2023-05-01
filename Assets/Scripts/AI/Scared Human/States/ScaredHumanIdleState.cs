using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredHumanIdleState : ScaredHumanBaseState
{
    public ScaredHumanIdleState(ScaredHuman context, ScaredHumanStateFactory factory) : base(context, factory) { }

    public override void OnEnter(ScaredHuman context)
    {
        
    }

    public override void OnUpdate(ScaredHuman context)
    {
        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(ScaredHuman context)
    {
        if (context.CanISeeMyTarget)
        {
            SwitchState(Factory.Retreat());
        }
    }

    public override void OnExit(ScaredHuman context)
    {
        
    }

    public override void InitializeSubState(ScaredHuman context)
    {

    }

}
