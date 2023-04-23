using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateFactory : MonoBehaviour
{
    enum EnemyStates
    {
        Idle,
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

    public EnemyStateFactory(SimpleEnemy currentContext)
    {
        _context = currentContext;
        _states = new Dictionary<EnemyStates, EnemyBaseState>();

        _states.Add(EnemyStates.Idle, new EnemyIdleState(_context, this));
        _states.Add(EnemyStates.Patrol, new EnemyPatrolState(_context, this));
        _states.Add(EnemyStates.Chase, new EnemyChaseState(_context, this));
        _states.Add(EnemyStates.Attack, new EnemyAttackState(_context, this));
        _states.Add(EnemyStates.Stun, new EnemyStunState(_context, this));
        _states.Add(EnemyStates.Grounded, new EnemyGroundedState(_context, this));
        _states.Add(EnemyStates.Falling, new EnemyFallingState(_context, this));
        _states.Add(EnemyStates.Jumping, new EnemyJumpingState(_context, this));
    }

    //States
    public EnemyBaseState Idle()
    {
        return _states[EnemyStates.Idle];
    }
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
