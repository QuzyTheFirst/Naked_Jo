using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerController player, PlayerStateFactory factory) : base(player, factory) { }

    public override void OnEnter(PlayerController player)
    {
        player.RigidBody.sharedMaterial = player.FullFrictionMat;
        player.Col.sharedMaterial = player.FullFrictionMat;

        InitializeSubState(player);
    }

    public override void OnUpdate(PlayerController player)
    {
        float currentSpeed = player.Velocity.x;

        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = player.WalkDirection * player.MaxSpeed;
        targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, player.LerpAmount);

        float speedDif = targetSpeed - currentSpeed;

        float accelRate = targetSpeed == 0 ? player.Deceleration : player.Acceleration;

        player.Velocity = new Vector2(player.Velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / player.RigidBody.mass, player.Velocity.y);

        CheckSwitchStates(player);
    }

    public override void InitializeSubState(PlayerController player)
    {
        
    }

    public override void CheckSwitchStates(PlayerController player)
    {
        if (player.IsWalking)
        {
            SwitchState(Factory.Walk());
        }

        if (player.DoRoll)
        {
            SwitchState(Factory.Rolling());
        }
    }

    public override void OnExit(PlayerController player)
    {
        player.RigidBody.sharedMaterial = player.FrictionLessMat;
        player.Col.sharedMaterial = player.FrictionLessMat;
        //Debug.Log("Exit Idle State");
    }
}

