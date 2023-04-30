using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyStateFactory : MonoBehaviour
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
    private Dictionary<EnemyStates, EnemyBaseState> _states;

    public SimpleEnemyStateFactory(SimpleEnemy currentContext)
    {
        _context = currentContext;
        _states = new Dictionary<EnemyStates, EnemyBaseState>();

        _states.Add(EnemyStates.Patrol, new SimpleEnemyPatrolState(_context, this));
        _states.Add(EnemyStates.Chase, new SimpleEnemyChaseState(_context, this));
        _states.Add(EnemyStates.Attack, new SimpleEnemyAttackState(_context, this));
        _states.Add(EnemyStates.Stun, new SimpleEnemyStunState(_context, this));
        _states.Add(EnemyStates.Grounded, new SimpleEnemyGroundedState(_context, this));
        _states.Add(EnemyStates.Falling, new SimpleEnemyFallingState(_context, this));
        _states.Add(EnemyStates.Jumping, new SimpleEnemyJumpingState(_context, this));
    }

    //States
    public EnemyBaseState Stun()
    {
        return _states[EnemyStates.Stun];
    }
    public EnemyBaseState Patrol()
    {
        return _states[EnemyStates.Patrol];
    }
    public EnemyBaseState Chase()
    {
        return _states[EnemyStates.Chase];
    }
    public EnemyBaseState Attack()
    {
        return _states[EnemyStates.Attack];
    }

    //Root States
    public EnemyBaseState Grounded()
    {
        return _states[EnemyStates.Grounded];
    }
    public EnemyBaseState Falling()
    {
        return _states[EnemyStates.Falling];
    }

    public EnemyBaseState Jumping()
    {
        return _states[EnemyStates.Jumping];
    }
}
