using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleEnemyBaseState
{
    private bool _isRootState = false;

    private SimpleEnemy _context;
    private SimpleEnemyStateFactory _factory;

    private SimpleEnemyBaseState _superState;
    private SimpleEnemyBaseState _subState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected SimpleEnemy Context { get { return _context; } }
    protected SimpleEnemyStateFactory Factory { get { return _factory; } }

    public SimpleEnemyBaseState SuperState { get { return _superState; } }
    public SimpleEnemyBaseState SubState { get { return _subState; } }

    public SimpleEnemyBaseState(SimpleEnemy context, SimpleEnemyStateFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public abstract void OnEnter(SimpleEnemy context);

    public abstract void OnUpdate(SimpleEnemy context);

    public abstract void CheckSwitchStates(SimpleEnemy context);

    public abstract void OnExit(SimpleEnemy context);

    public abstract void InitializeSubState(SimpleEnemy context);

    protected void SwitchState(SimpleEnemyBaseState newState)
    {
        OnExit(_context);

        newState.OnEnter(_context);

        if (_isRootState == true)
            _context.CurrentState = newState;
        else if (_superState != null)
            _superState.SetSubState(newState);
    }

    public void UpdateStates(SimpleEnemy context)
    {
        OnUpdate(context);
        if (_subState != null)
        {
            _subState.UpdateStates(context);
        }
    }

    protected void SetSuperState(SimpleEnemyBaseState state)
    {
        //Debug.Log($"New Super State is {state.GetType()}");
        _superState = state;
    }

    protected void SetSubState(SimpleEnemyBaseState state)
    {
        //Debug.Log($"New Sub State is {state.GetType()}");
        _subState = state;
        _subState.OnEnter(Context);
        state.SetSuperState(this);
    }

    // Delete It Later!!!
    public SimpleEnemyBaseState GetSubState()
    {
        return _subState;
    }
}
