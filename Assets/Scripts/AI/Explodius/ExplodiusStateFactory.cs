using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodiusStateFactory
{
    enum ExplodiusStates
    {
        Patrol,
        Chase,
        Explode,
        Grounded,
        Falling,
        Jumping,
    }

    private Explodius _context;
    private Dictionary<ExplodiusStates, ExplodiusBaseState> _states;

    public ExplodiusStateFactory(Explodius currentContext)
    {
        _context = currentContext;
        _states = new Dictionary<ExplodiusStates, ExplodiusBaseState>();

        _states.Add(ExplodiusStates.Patrol, new ExplodiusPatrolState(_context, this));
        _states.Add(ExplodiusStates.Chase, new ExplodiusChaseState(_context, this));
        _states.Add(ExplodiusStates.Explode, new ExplodiusExplodeState(_context, this));
        _states.Add(ExplodiusStates.Grounded, new ExplodiusGroundedState(_context, this));
        _states.Add(ExplodiusStates.Falling, new ExplodiusFallingState(_context, this));
        _states.Add(ExplodiusStates.Jumping, new ExplodiusJumpingState(_context, this));
    }

    //States
    public ExplodiusBaseState Patrol()
    {
        return _states[ExplodiusStates.Patrol];
    }
    public ExplodiusBaseState Chase()
    {
        return _states[ExplodiusStates.Chase];
    }
    public ExplodiusBaseState Explode()
    {
        return _states[ExplodiusStates.Explode];
    }

    //Root States
    public ExplodiusBaseState Grounded()
    {
        return _states[ExplodiusStates.Grounded];
    }
    public ExplodiusBaseState Falling()
    {
        return _states[ExplodiusStates.Falling];
    }

    public ExplodiusBaseState Jumping()
    {
        return _states[ExplodiusStates.Jumping];
    }
}
