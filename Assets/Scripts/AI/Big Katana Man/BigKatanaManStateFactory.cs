using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigKatanaManStateFactory
{
    enum BigKatanaManStates
    {
        Patrol,
        Idle,
        Chase,
        Attack,
        Stun,
        BulletsDeflect,
        Rolling,

        Grounded,
        Falling,
        Jumping,
    }

    private BigKatanaMan _context;
    private Dictionary<BigKatanaManStates, BigKatanaManBaseState> _states;

    public BigKatanaManStateFactory(BigKatanaMan currentContext)
    {
        _context = currentContext;
        _states = new Dictionary<BigKatanaManStates, BigKatanaManBaseState>();

        _states.Add(BigKatanaManStates.Patrol, new BigKatanaManPatrolState(_context, this));
        _states.Add(BigKatanaManStates.Idle, new BigKatanaManIdleState(_context, this));
        _states.Add(BigKatanaManStates.Chase, new BigKatanaManChaseState(_context, this));
        _states.Add(BigKatanaManStates.Attack, new BigKatanaManAttackState(_context, this));
        _states.Add(BigKatanaManStates.Stun, new BigKatanaManStunState(_context, this));
        _states.Add(BigKatanaManStates.BulletsDeflect, new BigKatanaManBulletDeflectState(_context, this));
        _states.Add(BigKatanaManStates.Rolling, new BigKatanaManRollingState(_context, this));

        _states.Add(BigKatanaManStates.Grounded, new BigKatanaManGroundedState(_context, this));
        _states.Add(BigKatanaManStates.Falling, new BigKatanaManFallingState(_context, this));
        _states.Add(BigKatanaManStates.Jumping, new BigKatanaManJumpingState(_context, this));
    }

    //States
    public BigKatanaManBaseState Stun()
    {
        return _states[BigKatanaManStates.Stun];
    }
    public BigKatanaManBaseState Patrol()
    {
        return _states[BigKatanaManStates.Patrol];
    }
    public BigKatanaManBaseState Idle()
    {
        return _states[BigKatanaManStates.Idle];
    }
    public BigKatanaManBaseState Chase()
    {
        return _states[BigKatanaManStates.Chase];
    }
    public BigKatanaManBaseState Attack()
    {
        return _states[BigKatanaManStates.Attack];
    }
    public BigKatanaManBaseState BulletsDeflect()
    {
        return _states[BigKatanaManStates.BulletsDeflect];
    }
    public BigKatanaManBaseState Rolling()
    {
        return _states[BigKatanaManStates.Rolling];
    }


    //Root States
    public BigKatanaManBaseState Grounded()
    {
        return _states[BigKatanaManStates.Grounded];
    }
    public BigKatanaManBaseState Falling()
    {
        return _states[BigKatanaManStates.Falling];
    }

    public BigKatanaManBaseState Jumping()
    {
        return _states[BigKatanaManStates.Jumping];
    }
}
