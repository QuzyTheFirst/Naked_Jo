using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerBaseState
{
    private bool _isRootState = false;

    private PlayerController _player;
    private PlayerStateFactory _factory;

    private PlayerBaseState _superState;
    private PlayerBaseState _subState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected PlayerController Player { get { return _player; } }
    protected PlayerStateFactory Factory { get { return _factory; } }

    public PlayerBaseState(PlayerController player, PlayerStateFactory factory)
    {
        _player = player;
        _factory = factory;
    }

    public abstract void OnEnter(PlayerController player);

    public abstract void OnUpdate(PlayerController player);

    public abstract void CheckSwitchStates(PlayerController player);

    public abstract void OnExit(PlayerController player);

    public abstract void InitializeSubState(PlayerController player);

    protected void SwitchState(PlayerBaseState newState)
    {
        OnExit(_player);

        newState.OnEnter(_player);

        if (_isRootState == true)
            _player.CurrentState = newState;
        else if (_superState != null)
            _superState.SetSubState(newState);
    }

    public void UpdateStates(PlayerController player)
    {
        OnUpdate(player);
        if(_subState != null)
        {
            _subState.UpdateStates(player);
        }
    }

    protected void SetSuperState(PlayerBaseState state)
    {
        //Debug.Log($"New Super State is {state.GetType()}");
        _superState = state;
    }

    protected void SetSubState(PlayerBaseState state)
    {
        //Debug.Log($"New Sub State is {state.GetType()}");
        _subState = state;
        state.SetSuperState(this);
    }

    // Delete It Later!!!
    public PlayerBaseState GetSubState()
    {
        return _subState;
    }
}

