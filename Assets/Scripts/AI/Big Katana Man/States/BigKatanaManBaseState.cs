using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BigKatanaManBaseState
{
    private bool _isRootState = false;

    private BigKatanaMan _context;
    private BigKatanaManStateFactory _factory;

    private BigKatanaManBaseState _superState;
    private BigKatanaManBaseState _subState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected BigKatanaMan Context { get { return _context; } }
    protected BigKatanaManStateFactory Factory { get { return _factory; } }

    public BigKatanaManBaseState SuperState { get { return _superState; } }
    public BigKatanaManBaseState SubState { get { return _subState; } }

    public BigKatanaManBaseState(BigKatanaMan context, BigKatanaManStateFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public abstract void OnEnter(BigKatanaMan context);

    public abstract void OnUpdate(BigKatanaMan context);

    public abstract void CheckSwitchStates(BigKatanaMan context);

    public abstract void OnExit(BigKatanaMan context);

    public abstract void InitializeSubState(BigKatanaMan context);

    protected void SwitchState(BigKatanaManBaseState newState)
    {
        OnExit(_context);

        newState.OnEnter(_context);

        if (_isRootState == true)
            _context.CurrentState = newState;
        else if (_superState != null)
            _superState.SetSubState(newState);
    }

    public void UpdateStates(BigKatanaMan context)
    {
        OnUpdate(context);
        if (_subState != null)
        {
            _subState.UpdateStates(context);
        }
    }

    protected void SetSuperState(BigKatanaManBaseState state)
    {
        //Debug.Log($"New Super State is {state.GetType()}");
        _superState = state;
    }

    protected void SetSubState(BigKatanaManBaseState state)
    {
        //Debug.Log($"New Sub State is {state.GetType()}");
        _subState = state;
        _subState.OnEnter(Context);
        state.SetSuperState(this);
    }

    // Delete It Later!!!
    public BigKatanaManBaseState GetSubState()
    {
        return _subState;
    }
}
