using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerBaseState
{
    public PlayerWalkingState(PlayerController player, PlayerStateFactory factory) : base(player, factory) { }

    public override void OnEnter(PlayerController player)
    {
        //Debug.Log("Entered Walking State");
        InitializeSubState(player);
    }

    public override void OnUpdate(PlayerController player)
    {
        float currentSpeed = player.Velocity.x;

        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = player.WalkDirection * player.MaxSpeed * player.MaxSpeedModifier;
        targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, player.LerpAmount);

        float speedDif = targetSpeed - currentSpeed;

        float accelRate = targetSpeed == 0 ? player.Deceleration : player.Acceleration;

        player.Velocity = new Vector2(player.Velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / player.MyRigidbody.mass, player.Velocity.y);

        CheckSwitchStates(player);
    }

    public override void CheckSwitchStates(PlayerController player)
    {
        if (!player.IsWalking)
        {
            SwitchState(Factory.Idle());
        }

        if(player.DoRoll)
        {
            SwitchState(Factory.Rolling());
        }
    }

    public override void InitializeSubState(PlayerController player)
    {
        
    }

    public override void OnExit(PlayerController player)
    {

    }
}
