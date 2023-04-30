using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerController player, PlayerStateFactory factory) : base(player, factory) 
    {
        IsRootState = true;
    }

    public override void OnEnter(PlayerController player)
    {
        InitializeSubState(player);
    }

    public override void OnUpdate(PlayerController player)
    {
        CheckSwitchStates(player);
    }

    public override void CheckSwitchStates(PlayerController player)
    {
        if (!player.IsGrounded)
        {
            SwitchState(Factory.Jump());
            return;
        }

        if(player.JumpPressedRemember > 0)
        {
            SwitchState(Factory.Jump());
            return;
        }
    }

    public override void InitializeSubState(PlayerController player)
    {
        if(!player.IsWalking)
        {
            SetSubState(Factory.Idle());
        }
        else if(player.IsWalking)
        {
            SetSubState(Factory.Walk());
        }
    }

    public override void OnExit(PlayerController player)
    {
        
    }
}
