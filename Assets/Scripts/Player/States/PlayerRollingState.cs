using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollingState : PlayerBaseState
{
    public PlayerRollingState(PlayerController player, PlayerStateFactory factory) : base(player, factory) { }

    float _time;

    Vector2 _oldVelocity;

    public override void OnEnter(PlayerController player)
    {
        //Debug.Log("Entered Rolling State");

        _time = player.RollingDistance / player.RollingSpeed;
        //Debug.Log($"Time: {_time}, Distance: {player.RollingDistance}, Speed: {player.RollingSpeed}");

        _oldVelocity = player.Velocity;
        player.Velocity = Vector2.zero;

        EnableIFrames();

        InitializeSubState(player);
    }

    public override void OnUpdate(PlayerController player)
    {
        if(_time > 0f && !ObstaclesCheck(player)&& player.DoRoll)
        {
            player.Velocity = Vector2.right * player.RollingDirection * player.RollingSpeed;
            _time -= Time.fixedDeltaTime;
        }
        else
        {
            player.DoRoll = false;
        }

        CheckSwitchStates(player);
    }

    public override void CheckSwitchStates(PlayerController player)
    {
        if (player.DoRoll)
            return;

        if (player.IsWalking)
        {
            SwitchState(Factory.Walk());
            return;
        }

        if (!player.IsWalking)
        {
            SwitchState(Factory.Idle());
            return;
        }
    }

    public override void InitializeSubState(PlayerController player)
    {

    }

    public override void OnExit(PlayerController player)
    {
        DisableIFrames();

        player.Velocity = _oldVelocity;
    }

    private bool ObstaclesCheck(PlayerController player)
    {
        RaycastHit2D wallHit = Physics2D.Raycast(player.transform.position, Vector2.right * player.RollingDirection, 1f, player.RollingObstacles);
        if (wallHit.transform != null)
            return true;

        RaycastHit2D groundHit = Physics2D.Raycast(player.transform.position, Vector2.down, 1f, player.RollingObstacles);
        if (groundHit.transform == null)
            return true;

        return false;
    }

    private void EnableIFrames()
    {
        Physics2D.IgnoreLayerCollision(8, 6, true);
        Physics2D.IgnoreLayerCollision(8, 7, true);
    }

    private void DisableIFrames()
    {
        Physics2D.IgnoreLayerCollision(8, 6, false);
        Physics2D.IgnoreLayerCollision(8, 7, false);
    }
}
