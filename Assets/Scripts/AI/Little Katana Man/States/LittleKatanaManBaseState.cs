using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LittleKatanaManBaseState
{
    private bool _isRootState = false;

    private LittleKatanaMan _context;
    private LittleKatanaManStateFactory _factory;

    private LittleKatanaManBaseState _superState;
    private LittleKatanaManBaseState _subState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected LittleKatanaMan Context { get { return _context; } }
    protected LittleKatanaManStateFactory Factory { get { return _factory; } }

    public LittleKatanaManBaseState SuperState { get { return _superState; } }
    public LittleKatanaManBaseState SubState { get { return _subState; } }

    public LittleKatanaManBaseState(LittleKatanaMan context, LittleKatanaManStateFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public abstract void OnEnter(LittleKatanaMan context);

    public abstract void OnUpdate(LittleKatanaMan context);

    public abstract void CheckSwitchStates(LittleKatanaMan context);

    public abstract void OnExit(LittleKatanaMan context);

    public abstract void InitializeSubState(LittleKatanaMan context);

    protected void SwitchState(LittleKatanaManBaseState newState)
    {
        OnExit(_context);

        newState.OnEnter(_context);

        if (_isRootState == true)
            _context.CurrentState = newState;
        else if (_superState != null)
            _superState.SetSubState(newState);
    }

    public void UpdateStates(LittleKatanaMan context)
    {
        OnUpdate(context);
        if (_subState != null)
        {
            _subState.UpdateStates(context);
        }
    }

    protected void SetSuperState(LittleKatanaManBaseState state)
    {
        //Debug.Log($"New Super State is {state.GetType()}");
        _superState = state;
    }

    protected void SetSubState(LittleKatanaManBaseState state)
    {
        //Debug.Log($"New Sub State is {state.GetType()}");
        _subState = state;
        _subState.OnEnter(Context);
        state.SetSuperState(this);
    }

    // Delete It Later!!!
    public LittleKatanaManBaseState GetSubState()
    {
        return _subState;
    }
}
