using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExplodiusBaseState
{
    private bool _isRootState = false;

    private Explodius _context;
    private ExplodiusStateFactory _factory;

    private ExplodiusBaseState _superState;
    private ExplodiusBaseState _subState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected Explodius Context { get { return _context; } }
    protected ExplodiusStateFactory Factory { get { return _factory; } }

    public ExplodiusBaseState SuperState { get { return _superState; } }
    public ExplodiusBaseState SubState { get { return _subState; } }

    public ExplodiusBaseState(Explodius context, ExplodiusStateFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public abstract void OnEnter(Explodius context);

    public abstract void OnUpdate(Explodius context);

    public abstract void CheckSwitchStates(Explodius context);

    public abstract void OnExit(Explodius context);

    public abstract void InitializeSubState(Explodius context);

    protected void SwitchState(ExplodiusBaseState newState)
    {
        OnExit(_context);

        newState.OnEnter(_context);

        if (_isRootState == true)
            _context.CurrentState = newState;
        else if (_superState != null)
            _superState.SetSubState(newState);
    }

    public void UpdateStates(Explodius context)
    {
        OnUpdate(context);
        if (_subState != null)
        {
            _subState.UpdateStates(context);
        }
    }

    protected void SetSuperState(ExplodiusBaseState state)
    {
        //Debug.Log($"New Super State is {state.GetType()}");
        _superState = state;
    }

    protected void SetSubState(ExplodiusBaseState state)
    {
        //Debug.Log($"New Sub State is {state.GetType()}");
        _subState = state;
        _subState.OnEnter(Context);
        state.SetSuperState(this);
    }

    // Delete It Later!!!
    public ExplodiusBaseState GetSubState()
    {
        return _subState;
    }
}
