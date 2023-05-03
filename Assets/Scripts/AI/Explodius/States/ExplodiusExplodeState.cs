using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodiusExplodeState : ExplodiusBaseState
{
    public ExplodiusExplodeState(Explodius context, ExplodiusStateFactory factory) : base(context, factory) { }

    public override void OnEnter(Explodius context)
    {
        context.Movement = AIBase.MovementState.Stop;
    }

    public override void OnUpdate(Explodius context)
    {
        context.ExplodeTimer -= Time.fixedDeltaTime;
        if(context.ExplodeTimer <= 0f)
        {
            Explode(context);
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

    private void Explode(Explodius context)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(context.transform.position, context.ExplosionRadius);
        foreach (Collider2D col in colls)
        {
            Unit unit = col.GetComponent<Unit>();
            if (unit != null)
            {
                if (unit == context.MyUnit)
                    continue;

                unit.Damage(100f);
            }
        }

        context.MyUnit.Damage(100f);
    }

}
