using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SturdyDashState : SturdyBaseState
{
    public SturdyDashState(Sturdy context, SturdyStateFactory factory) : base(context, factory) { }

    public override void OnEnter(Sturdy context)
    {

    }

    public override void OnUpdate(Sturdy context)
    {

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(Sturdy context)
    {

    }

    public override void OnExit(Sturdy context)
    {

    }

    public override void InitializeSubState(Sturdy context)
    {

    }
}
