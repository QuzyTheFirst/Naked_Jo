using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SturdyStateFactory
{
    enum SturdyStates
    {
        Patrol,
        Chase,
        Attack,
        Dash,
        Stun,

        Grounded,
        Falling,
        Jumping,
    }

    private Sturdy _context;
    private Dictionary<SturdyStates, SturdyBaseState> _states;

    public SturdyStateFactory(Sturdy currentContext)
    {
        _context = currentContext;
        _states = new Dictionary<SturdyStates, SturdyBaseState>();

        _states.Add(SturdyStates.Patrol, new SturdyPatrolState(_context, this));
        _states.Add(SturdyStates.Chase, new SturdyChaseState(_context, this));
        _states.Add(SturdyStates.Attack, new SturdyAttackState(_context, this));
        _states.Add(SturdyStates.Dash, new SturdyDashState(_context, this));
        _states.Add(SturdyStates.Stun, new SturdyStunState(_context, this));

        _states.Add(SturdyStates.Grounded, new SturdyGroundedState(_context, this));
        _states.Add(SturdyStates.Falling, new SturdyFallingState(_context, this));
        _states.Add(SturdyStates.Jumping, new SturdyJumpingState(_context, this));
    }

    //States
    public SturdyBaseState Patrol()
    {
        return _states[SturdyStates.Patrol];
    }
    public SturdyBaseState Chase()
    {
        return _states[SturdyStates.Chase];
    }
    public SturdyBaseState Attack()
    {
        return _states[SturdyStates.Attack];
    }
    public SturdyBaseState Dash()
    {
        return _states[SturdyStates.Dash];
    }
    public SturdyBaseState Stun()
    {
        return _states[SturdyStates.Stun];
    }

    //Root States
    public SturdyBaseState Grounded()
    {
        return _states[SturdyStates.Grounded];
    }
    public SturdyBaseState Falling()
    {
        return _states[SturdyStates.Falling];
    }

    public SturdyBaseState Jumping()
    {
        return _states[SturdyStates.Jumping];
    }
}
