using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredHumanStateFactory
{
    enum ScaredHumanStates
    {
        Idle,
        Retreat,
        Stun,
        Grounded,
        Falling,
    }

    private ScaredHuman _context;
    private Dictionary<ScaredHumanStates, ScaredHumanBaseState> _states;

    public ScaredHumanStateFactory(ScaredHuman currentContext)
    {
        _context = currentContext;
        _states = new Dictionary<ScaredHumanStates, ScaredHumanBaseState>();

        _states.Add(ScaredHumanStates.Idle, new ScaredHumanIdleState(_context, this));
        _states.Add(ScaredHumanStates.Retreat, new ScaredHumanRetreatState(_context, this));
        _states.Add(ScaredHumanStates.Stun, new ScaredHumanStunState(_context, this));
        _states.Add(ScaredHumanStates.Grounded, new ScaredHumanGroundedState(_context, this));
        _states.Add(ScaredHumanStates.Falling, new ScaredHumanFallingState(_context, this));

        Debug.Log(_states[ScaredHumanStates.Grounded]);
    }

    public ScaredHumanBaseState Idle()
    {
        return _states[ScaredHumanStates.Idle];
    }
    public ScaredHumanBaseState Retreat()
    {
        return _states[ScaredHumanStates.Retreat];
    }
    public ScaredHumanBaseState Stun()
    {
        return _states[ScaredHumanStates.Stun];
    }
    public ScaredHumanBaseState Grounded()
    {
        return _states[ScaredHumanStates.Grounded];
    }
    public ScaredHumanBaseState Falling()
    {
        return _states[ScaredHumanStates.Falling];
    }
}
