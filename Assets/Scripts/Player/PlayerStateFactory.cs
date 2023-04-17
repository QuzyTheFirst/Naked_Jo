using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory : MonoBehaviour
{
    enum PlayerStates
    {
        Idle,
        Walking,
        Jump,
        Grounded,
        Rolling,
    }

    private PlayerController _context;
    private Dictionary<PlayerStates, PlayerBaseState> _states;

    public PlayerStateFactory(PlayerController currentContext)
    {
        _context = currentContext;
        _states = new Dictionary<PlayerStates, PlayerBaseState>();

        _states.Add(PlayerStates.Idle, new PlayerIdleState(_context, this));
        _states.Add(PlayerStates.Walking, new PlayerWalkingState(_context, this));
        _states.Add(PlayerStates.Jump, new PlayerJumpState(_context, this));
        _states.Add(PlayerStates.Grounded, new PlayerGroundedState(_context, this));
        _states.Add(PlayerStates.Rolling, new PlayerRollingState(_context, this));
    }

    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.Idle];
    }

    public PlayerBaseState Walk()
    {
        return _states[PlayerStates.Walking];
    }

    public PlayerBaseState Jump()
    {
        return _states[PlayerStates.Jump];
    }

    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.Grounded];
    }

    public PlayerBaseState Rolling()
    {
        return _states[PlayerStates.Rolling];
    }
}

