using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SturdyRollingState : SturdyBaseState
{
    public SturdyRollingState(Sturdy context, SturdyStateFactory factory) : base(context, factory) { }

    float _time;

    Vector2 _oldVelocity;

    public override void OnEnter(Sturdy context)
    {
        _time = context.MyPlayerController.RollingDistance / context.MyPlayerController.RollingSpeed;
        //Debug.Log($"Time: {_time}, Distance: {context.MyPlayerController.RollingDistance}, Speed: {context.MyPlayerController.RollingSpeed}");

        _oldVelocity = context.MyRigidbody.velocity;
        context.MyRigidbody.velocity = Vector2.zero;

        EnableIFrames();

        InitializeSubState(context);
    }

    public override void OnUpdate(Sturdy context)
    {
        if (_time > 0f && !ObstaclesCheck(context) && context.DoRoll)
        {
            context.MyRigidbody.velocity = Vector2.right * context.RollingDirection * context.MyPlayerController.RollingSpeed;
            _time -= Time.fixedDeltaTime;
        }
        else
        {
            context.DoRoll = false;
        }

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(Sturdy context)
    {
        if (context.DoRoll)
            return;

        if (context.StunTime > 0f)
        {
            SwitchState(Factory.Stun());
            return;
        }

        if (context.ChasePlayerAfterDissapearanceTimer <= 0f || context.TargetUnit == null)
        {
            SwitchState(Factory.Patrol());
            return;
        }

        if (context.ChasePlayerAfterDissapearanceTimer > 0f || context.TargetUnit == null)
        {
            SwitchState(Factory.Chase());
            return;
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if (distance < context.AttackRadius && context.CanISeeMyTarget)
        {
            SwitchState(Factory.Attack());
            return;
        }
    }

    public override void OnExit(Sturdy context)
    {
        DisableIFrames();

        context.MyRigidbody.velocity = _oldVelocity;
    }

    public override void InitializeSubState(Sturdy context)
    {

    }
    private bool ObstaclesCheck(Sturdy context)
    {
        RaycastHit2D wallHit = Physics2D.Raycast(context.transform.position, Vector2.right * context.RollingDirection, 1f, context.MyPlayerController.RollingObstacles);
        if (wallHit.transform != null)
            return true;

        RaycastHit2D groundHit = Physics2D.Raycast(context.transform.position, Vector2.down, 1f, context.MyPlayerController.RollingObstacles);
        if (groundHit.transform == null)
            return true;

        return false;
    }

    private void EnableIFrames()
    {
        Physics2D.IgnoreLayerCollision(7, 6, true);
        Physics2D.IgnoreLayerCollision(7, 8, true);
        Physics2D.IgnoreLayerCollision(7, 9, true);
    }

    private void DisableIFrames()
    {
        Physics2D.IgnoreLayerCollision(7, 6, false);
        Physics2D.IgnoreLayerCollision(7, 8, false);
        Physics2D.IgnoreLayerCollision(7, 9, false);
    }
}
