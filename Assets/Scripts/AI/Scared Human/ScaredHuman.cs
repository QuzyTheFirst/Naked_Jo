using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredHuman : AIBase
{
    // Simple Enemy States
    private ScaredHumanBaseState _currentState;
    private ScaredHumanStateFactory _states;

    public ScaredHumanBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    private void Start()
    {
        _states = new ScaredHumanStateFactory(this);
        _currentState = _states.Grounded();

        _currentState.OnEnter(this);
    }

    protected override void FixedUpdate()
    {
        if (IsPossessed)
            return;

        base.FixedUpdate();

        _currentState.UpdateStates(this);
        Debug.Log($"Current State: {_currentState} | Current Sub State: {_currentState.GetSubState()}");
    }

    public override void Stun(float time)
    {
        base.Stun(time);

        _currentState.UpdateStates(this);
    }
}
