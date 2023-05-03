using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodius : AIBase
{
    [Header("Explodius")]

    [Header("Chase State")]
    [SerializeField] private float _chasePlayerAfterDissapearanceTime = 5f;
    private float _chasePlayerAfterDissapearanceTimer;

    [Header("Explosion State")]
    [SerializeField] private float _distanceToStartExplosion;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _timeToExplode = .5f;
    private float _explodeTimer;

    // Simple Enemy States
    private ExplodiusBaseState _currentState;
    private ExplodiusStateFactory _states;

    //States
    public ExplodiusBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    //Chase State
    public float ChasePlayerAfterDissapearanceTime { get { return _chasePlayerAfterDissapearanceTime; } }
    public float ChasePlayerAfterDissapearanceTimer { get { return _chasePlayerAfterDissapearanceTimer; } set { _chasePlayerAfterDissapearanceTimer = value; } }

    //Explosion State
    public float DistanceToStartExplosion { get { return _distanceToStartExplosion; } }
    public float ExplosionRadius { get { return _explosionRadius; } set { _explosionRadius = value; } }
    public float ExplodeTimer { get { return _explodeTimer; } set { _explodeTimer = value; } }

    private void Start()
    {
        _states = new ExplodiusStateFactory(this);
        _currentState = _states.Grounded();

        _currentState.OnEnter(this);

        _explodeTimer = _timeToExplode;
    }

    protected override void FixedUpdate()
    {
        if (IsPossessed)
            return;

        base.FixedUpdate();

        if (CanISeeMyTarget)
        {
            _chasePlayerAfterDissapearanceTimer = _chasePlayerAfterDissapearanceTime;
        }
        else
        {
            _chasePlayerAfterDissapearanceTimer -= Time.fixedDeltaTime;
        }

        SearchForNewWeapon();

        _currentState.UpdateStates(this);
        Debug.Log($"Current State: {_currentState} | Current Sub State: {_currentState.GetSubState()}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _distanceToStartExplosion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
