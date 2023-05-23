using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleKatanaManStateFactory
{
    enum LittleKatanaManStates
    {
        Patrol,
        Idle,
        Chase,
        Attack,
        Stun,
        BulletsDeflect,

        Grounded,
        Falling,
        Jumping,
    }

    private LittleKatanaMan _context;
    private Dictionary<LittleKatanaManStates, LittleKatanaManBaseState> _states;

    public LittleKatanaManStateFactory(LittleKatanaMan currentContext)
    {
        _context = currentContext;
        _states = new Dictionary<LittleKatanaManStates, LittleKatanaManBaseState>();

        _states.Add(LittleKatanaManStates.Patrol, new LittleKatanaManPatrolState(_context, this));
        _states.Add(LittleKatanaManStates.Idle, new LittleKatanaManIdleState(_context, this));
        _states.Add(LittleKatanaManStates.Chase, new LittleKatanaManChaseState(_context, this));
        _states.Add(LittleKatanaManStates.Attack, new LittleKatanaManAttackState(_context, this));
        _states.Add(LittleKatanaManStates.Stun, new LittleKatanaManStunState(_context, this));
        _states.Add(LittleKatanaManStates.BulletsDeflect, new LittleKatanaManBulletDeflectState(_context, this));

        _states.Add(LittleKatanaManStates.Grounded, new LittleKatanaManGroundedState(_context, this));
        _states.Add(LittleKatanaManStates.Falling, new LittleKatanaManFallingState(_context, this));
        _states.Add(LittleKatanaManStates.Jumping, new LittleKatanaManJumpingState(_context, this));
    }

    //States
    public LittleKatanaManBaseState Stun()
    {
        return _states[LittleKatanaManStates.Stun];
    }
    public LittleKatanaManBaseState Patrol()
    {
        return _states[LittleKatanaManStates.Patrol];
    }
    public LittleKatanaManBaseState Idle()
    {
        return _states[LittleKatanaManStates.Idle];
    }
    public LittleKatanaManBaseState Chase()
    {
        return _states[LittleKatanaManStates.Chase];
    }
    public LittleKatanaManBaseState Attack()
    {
        return _states[LittleKatanaManStates.Attack];
    }
    public LittleKatanaManBaseState BulletsDeflect()
    {
        return _states[LittleKatanaManStates.BulletsDeflect];
    }


    //Root States
    public LittleKatanaManBaseState Grounded()
    {
        return _states[LittleKatanaManStates.Grounded];
    }
    public LittleKatanaManBaseState Falling()
    {
        return _states[LittleKatanaManStates.Falling];
    }

    public LittleKatanaManBaseState Jumping()
    {
        return _states[LittleKatanaManStates.Jumping];
    }
}
