using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit
{
    PlayerController _playerController;

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
}
