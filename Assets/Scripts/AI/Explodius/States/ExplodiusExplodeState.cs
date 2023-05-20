using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodiusExplodeState : ExplodiusBaseState
{
    public ExplodiusExplodeState(Explodius context, ExplodiusStateFactory factory) : base(context, factory) { }

    public override void OnEnter(Explodius context)
    {
        context.Movement = AIBase.MovementState.Stop;
        context.HasExplosionStarted = true;
    }

    public override void OnUpdate(Explodius context)
    {
        context.ExplodeTimer -= Time.fixedDeltaTime;

        LeanTween.color(context.MySpriteRenderer.gameObject, Color.black, context.TimeToExplode);

        if(context.ExplodeTimer <= 0f)
        {
            context.Explode(context);
        }
    }

    public override void CheckSwitchStates(Explodius context)
    {

    }

    public override void OnExit(Explodius context)
    {
        Debug.Log("You cannot escape");
    }

    public override void InitializeSubState(Explodius context)
    {

    }

}
