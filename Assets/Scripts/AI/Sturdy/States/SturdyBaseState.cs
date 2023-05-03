using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SturdyBaseState
{
    private bool _isRootState = false;

    private Sturdy _context;
    private SturdyStateFactory _factory;

    private SturdyBaseState _superState;
    private SturdyBaseState _subState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected Sturdy Context { get { return _context; } }
    protected SturdyStateFactory Factory { get { return _factory; } }

    public SturdyBaseState SuperState { get { return _superState; } }
    public SturdyBaseState SubState { get { return _subState; } }

    public SturdyBaseState(Sturdy context, SturdyStateFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public abstract void OnEnter(Sturdy context);

    public abstract void OnUpdate(Sturdy context);

    public abstract void CheckSwitchStates(Sturdy context);

    public abstract void OnExit(Sturdy context);

    public abstract void InitializeSubState(Sturdy context);

    protected void SwitchState(SturdyBaseState newState)
    {
        OnExit(_context);

        newState.OnEnter(_context);

        if (_isRootState == true)
            _context.CurrentState = newState;
        else if (_superState != null)
            _superState.SetSubState(newState);
    }

    public void UpdateStates(Sturdy context)
    {
        OnUpdate(context);
        if (_subState != null)
        {
            _subState.UpdateStates(context);
        }
    }

    protected void SetSuperState(SturdyBaseState state)
    {
        //Debug.Log($"New Super State is {state.GetType()}");
        _superState = state;
    }

    protected void SetSubState(SturdyBaseState state)
    {
        //Debug.Log($"New Sub State is {state.GetType()}");
        _subState = state;
        _subState.OnEnter(Context);
        state.SetSuperState(this);
    }

    // Delete It Later!!!
    public SturdyBaseState GetSubState()
    {
        return _subState;
    }
}
