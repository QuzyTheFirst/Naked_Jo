using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit
{
    PlayerController _playerController;

    public bool IsRolling { get { return _playerController.DoRoll; } }

    public bool CanRoll { get { return _playerController.RollingDistance > 0f; } }

    public PlayerUnit(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void SetMaxSpeedModifier(float value)
    {
        _playerController.MaxSpeedModifier = value;
    }

    public void MovePerformed(float value)
    {
        _playerController.MovePerformed(value);
    }

    public void MoveCanceled()
    {
        _playerController.MoveCanceled();
    }

    public void JumpPerformed()
    {
        _playerController.JumpPerformed();
    }

    public void JumpCanceled()
    {
        _playerController.JumpCanceled();
    }

    public bool Roll(float dir)
    {
        if (!_playerController.IsGrounded || !CanRoll)
            return false;

        _playerController.DoRoll = true;
        _playerController.RollingDirection = dir;
        return true;
    }
}
