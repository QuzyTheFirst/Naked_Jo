using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyStateFactory
{
    enum EnemyStates
    {
        Patrol,
        Chase,
        Attack,
        Stun,
        Grounded,
        Falling,
        Jumping,
    }

    private SimpleEnemy _context;
    private Dictionary<EnemyStates, SimpleEnemyBaseState> _states;

    public SimpleEnemyStateFactory(SimpleEnemy currentContext)
    {
        _context = currentContext;
        _states = new Dictionary<EnemyStates, SimpleEnemyBaseState>();

        _states.Add(EnemyStates.Patrol, new SimpleEnemyPatrolState(_context, this));
        _states.Add(EnemyStates.Chase, new SimpleEnemyChaseState(_context, this));
        _states.Add(EnemyStates.Attack, new SimpleEnemyAttackState(_context, this));
        _states.Add(EnemyStates.Stun, new SimpleEnemyStunState(_context, this));
        _states.Add(EnemyStates.Grounded, new SimpleEnemyGroundedState(_context, this));
        _states.Add(EnemyStates.Falling, new SimpleEnemyFallingState(_context, this));
        _states.Add(EnemyStates.Jumping, new SimpleEnemyJumpingState(_context, this));
    }

    //States
    public SimpleEnemyBaseState Stun()
    {
        return _states[EnemyStates.Stun];
    }
    public SimpleEnemyBaseState Patrol()
    {
        return _states[EnemyStates.Patrol];
    }
    public SimpleEnemyBaseState Chase()
    {
        return _states[EnemyStates.Chase];
    }
    public SimpleEnemyBaseState Attack()
    {
        return _states[EnemyStates.Attack];
    }

    //Root States
    public SimpleEnemyBaseState Grounded()
    {
        return _states[EnemyStates.Grounded];
    }
    public SimpleEnemyBaseState Falling()
    {
        return _states[EnemyStates.Falling];
    }

    public SimpleEnemyBaseState Jumping()
    {
        return _states[EnemyStates.Jumping];
    }
}
