using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodius : AIBase
{
    [Header("Explodius")]
    [SerializeField] private bool _startWithIdle = false;
    private bool _isExploding = false;

    [Header("Chase State")]
    [SerializeField] private float _chasePlayerAfterDissapearanceTime = 5f;
    private float _chasePlayerAfterDissapearanceTimer;

    [Header("Explosion State")]
    [SerializeField] private float _distanceToStartExplosion;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _timeToExplode = .5f;
    [SerializeField] private ParticleSystem _explosionParticles;
    private float _explodeTimer;

    private bool _hasExplosionStarted = false;

    // Simple Enemy States
    private ExplodiusBaseState _currentState;
    private ExplodiusStateFactory _states;

    //States
    public ExplodiusBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    //Idle State
    public bool StartWithIdle { get { return _startWithIdle; } set { _startWithIdle = value; } }

    //Chase State
    public float ChasePlayerAfterDissapearanceTime { get { return _chasePlayerAfterDissapearanceTime; } }
    public float ChasePlayerAfterDissapearanceTimer { get { return _chasePlayerAfterDissapearanceTimer; } set { _chasePlayerAfterDissapearanceTimer = value; } }

    //Explosion State
    public float DistanceToStartExplosion { get { return _distanceToStartExplosion; } }
    public float ExplosionRadius { get { return _explosionRadius; } set { _explosionRadius = value; } }
    public float ExplodeTimer { get { return _explodeTimer; } set { _explodeTimer = value; } }
    public float TimeToExplode { get { return _timeToExplode; } }
    public bool HasExplosionStarted { get { return _hasExplosionStarted; } set { _hasExplosionStarted = value; } }

    public bool IsExploding { get { return _isExploding; } }

    private void Start()
    {
        _states = new ExplodiusStateFactory(this);
        _currentState = _states.Grounded();

        _currentState.OnEnter(this);

        _explodeTimer = _timeToExplode;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_hasExplosionStarted && IsPossessed)
        {
            _currentState.UpdateStates(this);
        }

        if (IsPossessed)
            return;

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
        //Debug.Log($"Current State: {_currentState} | Current Sub State: {_currentState.GetSubState()}");
    }

    public override bool Damage(Vector2 from, int amount)
    {
        if (!_isExploding)
        {
            Explode(this);
            return false;
        }

        return true;
    }

    public void Explode(Explodius context)
    {
        if (_isExploding)
            return;

        _isExploding = true;
        context.MyUnit.HasExploded = true;

        Collider2D[] colls = Physics2D.OverlapCircleAll(context.transform.position, context.ExplosionRadius);
        foreach (Collider2D col in colls)
        {
            Unit unit = col.GetComponent<Unit>();
            if (unit != null)
            {
                if (unit == context.MyUnit)
                    continue;

                if (unit.MyEnemyController is Explodius)
                {
                    Explodius explodius = unit.MyEnemyController as Explodius;

                    if (explodius.IsExploding)
                        continue;
                }

                Rigidbody2D rig = col.transform.GetComponent<Rigidbody2D>();
                if(rig != null)
                {
                    Vector2 dir = (col.transform.position - transform.position).normalized;
                    rig.velocity = dir * 6;
                }

                unit.Damage(transform.position, 100);
            }
        }

        ParticleSystem particles = Instantiate(_explosionParticles);
        particles.transform.position = transform.position;
        particles.Play();

        context.MyUnit.Damage(transform.position, 100);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _distanceToStartExplosion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
