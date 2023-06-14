using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sturdy : AIBase
{
    [Header("Sturdy")]
    [SerializeField] private bool _startWithIdle = false;
    [SerializeField] private float _lookForDangerRadius = 3f;

    [Header("Chase State")]
    [SerializeField] private float _chasePlayerAfterDissapearanceTime = 5f;
    private float _chasePlayerAfterDissapearanceTimer;

    [Header("Helmet and Sprite")]
    [SerializeField] private Transform _sturdyHelmet;
    [SerializeField] private Sprite _sturdyWithoutHelmetPossessed;
    [SerializeField] private Sprite _sturdyWithoutHelmetNormalState;

    private bool _isDamaged = false;

    // Rolling
    [Header("Rolling")]
    [SerializeField] private float _rollingCooldown;
    private float _nextRollingTime;
    private bool _doRoll;
    private float _rollingDirection;

    // Simple Enemy States
    private SturdyBaseState _currentState;
    private SturdyStateFactory _states;

    public bool StartWithIdle { get { return _startWithIdle; } set { _startWithIdle = value; } }

    // Shoot
    private float _timeToNextShoot;

    public float TimeToNextShoot { get { return _timeToNextShoot; } set { _timeToNextShoot = value; } }

    public float ShootEvery { get { return MyWeaponController.GetWeaponParams().EnemyAttackRate; } }

    //States
    public SturdyBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    //Chase State
    public float ChasePlayerAfterDissapearanceTime { get { return _chasePlayerAfterDissapearanceTime; } }
    public float ChasePlayerAfterDissapearanceTimer { get { return _chasePlayerAfterDissapearanceTimer; } set { _chasePlayerAfterDissapearanceTimer = value; } }

    //Rolling State
    public bool isDangerAround { get { return CheckForDangerAround(); } }
    public float RollingDirection { get { return _rollingDirection; } set { _rollingDirection = value; } }
    public bool DoRoll { get { return _doRoll; } set { _doRoll = value; } }
    private void Start()
    {
        _states = new SturdyStateFactory(this);
        _currentState = _states.Grounded();

        _currentState.OnEnter(this);

        _nextRollingTime = Time.time;

        _sturdyHelmet.gameObject.SetActive(false);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

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

        if (Time.time > _nextRollingTime)
        {
            Collider2D danger = CheckForDangerAround();
            if (danger != null)
            {
                _rollingDirection = Mathf.Sign((danger.transform.position - transform.position).x);
                _doRoll = true;
                _nextRollingTime = Time.time + _rollingCooldown;
            }
        }

        _currentState.UpdateStates(this);

        //Debug.Log("Is it dangerous: " + CheckForDangerAround());
        //Debug.Log($"Current State: {_currentState} | Current Sub State: {_currentState.GetSubState()}");
    }


    public override void Possess(LayerMask attackMask)
    {
        base.Possess(attackMask);

        AttackMask = attackMask;
        MyWeaponController.SetAttackMask(attackMask);

        _chasePlayerAfterDissapearanceTimer = 0f;

        if (_isDamaged)
        {
            MySpriteRenderer.sprite = _sturdyWithoutHelmetPossessed;
        }
        else
        {
            MySpriteRenderer.sprite = _possesedStateSprite;
        }
    }

    public override void UnPossess(LayerMask attackMask, Transform targetUnit)
    {
        base.UnPossess(attackMask, targetUnit);

        AttackMask = attackMask;
        MyWeaponController.SetAttackMask(attackMask);

        if (_isDamaged)
        {
            MySpriteRenderer.sprite = _sturdyWithoutHelmetNormalState;
        }
        else
        {
            MySpriteRenderer.sprite = _normalStateSprite;
        }
    }

    public override void Stun(float time)
    {
        base.Stun(time);

        _currentState.UpdateStates(this);
    }

    public Collider2D CheckForDangerAround()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position, _lookForDangerRadius, LayerMask.GetMask("Bullet") + LayerMask.GetMask("FlyingWeapon"));
        if(coll != null)
        {
            return coll;
        }

        if (TargetUnit == null)
            return null;
         
        float distanceToPlayer = Vector2.Distance(transform.position, TargetUnitTf.position);
        if(distanceToPlayer <= _lookForDangerRadius)
        {
            if (TargetUnit.IsAttacking)
            {
                return TargetUnit.MyCircleCollider;
            }
        }

        return null;
    }

    public override bool Damage(Vector2 from, int amount)
    {
        if (!_isDamaged)
        {
            if (IsPossessed)
            {
                MySpriteRenderer.sprite = _sturdyWithoutHelmetPossessed;
            }
            else
            {
                MySpriteRenderer.sprite = _sturdyWithoutHelmetNormalState;
            }

            _sturdyHelmet.gameObject.SetActive(true);
            Vector2 dir = ((Vector2)transform.position - from).normalized;

            Rigidbody2D rig = _sturdyHelmet.GetComponent<Rigidbody2D>();
            rig.velocity = dir * 12;
            rig.angularVelocity = 720;

            _isDamaged = true;
        }

        return base.Damage(from, amount);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _lookForDangerRadius);
    }
}
