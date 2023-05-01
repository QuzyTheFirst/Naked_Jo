using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScaredHumanBaseState
{
    private bool _isRootState = false;

    private ScaredHuman _context;
    private ScaredHumanStateFactory _factory;

    private ScaredHumanBaseState _superState;
    private ScaredHumanBaseState _subState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected ScaredHuman Context { get { return _context; } }
    protected ScaredHumanStateFactory Factory { get { return _factory; } }

    public ScaredHumanBaseState SuperState { get { return _superState; } }
    public ScaredHumanBaseState SubState { get { return _subState; } }

    public ScaredHumanBaseState(ScaredHuman context, ScaredHumanStateFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public abstract void OnEnter(ScaredHuman context);

    public abstract void OnUpdate(ScaredHuman context);

    public abstract void CheckSwitchStates(ScaredHuman context);

    public abstract void OnExit(ScaredHuman context);

    public abstract void InitializeSubState(ScaredHuman context);

    protected void SwitchState(ScaredHumanBaseState newState)
    {
        OnExit(_context);

        newState.OnEnter(_context);

        if (_isRootState == true)
            _context.CurrentState = newState;
        else if (_superState != null)
            _superState.SetSubState(newState);
    }

    public void UpdateStates(ScaredHuman context)
    {
        OnUpdate(context);
        if (_subState != null)
        {
            _subState.UpdateStates(context);
        }
    }

    protected void SetSuperState(ScaredHumanBaseState state)
    {
        //Debug.Log($"New Super State is {state.GetType()}");
        _superState = state;
    }

    protected void SetSubState(ScaredHumanBaseState state)
    {
        //Debug.Log($"New Sub State is {state.GetType()}");
        _subState = state;
        _subState.OnEnter(Context);
        state.SetSuperState(this);
    }

    // Delete It Later!!!
    public ScaredHumanBaseState GetSubState()
    {
        return _subState;
    }
}
