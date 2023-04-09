using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private float _jumpPower = 0f;
    private bool _isJumping = false;
    public PlayerJumpState(PlayerController player, PlayerStateFactory factory) : base(player, factory)
    {
        IsRootState = true;
    }

    public override void OnEnter(PlayerController player)
    {
        InitializeSubState(player);
        //Debug.Log("Entered Jump State");
        if (player.JumpPressedRemember > 0)
        {
            JumpAction(player);
        }
    }

    public override void OnUpdate(PlayerController player)
    {
        if (player.JumpPressedRemember > 0)
        {
            JumpAction(player);
        }

        if (player.GroundRemember <= 0 && player.JumpPhase == 0)
            player.JumpPhase++;

        if (_isJumping && !player.IsJumpButtonPressed && player.Velocity.y > 0)
        {
            player.Velocity = new Vector2(player.Velocity.x, player.Velocity.y / 2);
            _isJumping = false;
        }
        else if(_isJumping && player.Velocity.y <= 0)
        {
            _isJumping = false;
        }

        if(player.RigidBody.velocity.y > 0)
        {
            player.RigidBody.gravityScale = player.UpwardMovementMultiplier;
        }
        else if (player.RigidBody.velocity.y < 0)
        {
            player.RigidBody.gravityScale = player.DownwardMovementMultiplier;
        }
        else if(player.RigidBody.velocity.y == 0)
        {
            player.RigidBody.gravityScale = player.DefaultGravityScale;
        }

        CheckSwitchStates(player);
    }

    private void JumpAction(PlayerController player)
    {
        if(player.GroundRemember > 0f || player.JumpPhase < player.MaxAirJumps)
        {
            player.JumpPhase++;

            SoundManager.Instance.Play("Jump");

            _jumpPower = Mathf.Sqrt(-2f * Physics.gravity.y * player.JumpHeight);

            player.GroundRemember = 0f;
            player.JumpPressedRemember = 0f;
            player.StepsSinceLastJump = 0;

            if (player.Velocity.y > 0f)
            {
                _jumpPower = Mathf.Max(_jumpPower - player.Velocity.y, 0f);
            }

            player.Velocity = Vector2.up * _jumpPower + Vector2.right * player.Velocity.x;

            _isJumping = true;
        }
    }

    public override void CheckSwitchStates(PlayerController player)
    {
        if (player.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void InitializeSubState(PlayerController player)
    {
        if (!player.IsWalking)
        {
            SetSubState(Factory.Idle());
        }
        else if (player.IsWalking)
        {
            SetSubState(Factory.Walk());
        }
    }

    public override void OnExit(PlayerController player)
    {
        player.RigidBody.gravityScale = player.DefaultGravityScale;
    }
}

