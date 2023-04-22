using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public enum MovementState
    {
        Left = -1,
        Stop = 0,
        Right = 1,
    }
    private MovementState _movement = MovementState.Right;

    private PlayerController _unitController;
    private WeaponController _weaponController;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rig;
    private CircleCollider2D _col;
    private HeadTrigger _headTrigger;

    private Flip _flip;

    private Unit _targetUnit;

    private bool _isPossessed = false;

    private float _stunTime;

    [Header("Weapon")]
    [SerializeField] private Transform _weaponToTake;
    [SerializeField] private LayerMask _attackMask;

    [Header("Shoot")]
    [SerializeField] private float _shootEvery = 1f;
    [SerializeField] private float _visionRadius;
    [SerializeField] private LayerMask _weaponMask;
    private float _timeToNextShoot;

    [SerializeField] private float _timeBeforeAction;
    private float _timerBeforeAction;

    [Header("Movement")]
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _lerpAmount;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;

    private float _movementSpeed;

    [Header("Ground Check")]
    [SerializeField] private float _checkDistance = .1f;
    [SerializeField] private LayerMask _groundMask;
    private bool _isGrounded = false;

    [Header("AI")]
    [SerializeField] private Sprite _possesedStateSprite;
    [SerializeField] private Sprite _normalStateSprite;
    [SerializeField] private GameObject _stunAnimGO;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private float _unpossessFlyPower;
    private float _attackRadius;

    // Enemy States
    private EnemyBaseState _currentState;
    private EnemyStateFactory _states;

    public EnemyBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public EnemyStateFactory States { get { return _states; } }
    public float MovementDirection { get { return (int)_movement; } }
    public float AttackRadius { get { return _attackRadius; } }
    public MovementState Movement { get { return _movement; } set { _movement = value; } }
    //------------
    public SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }
    public Rigidbody2D Rig { get { return _rig; } }
    public Flip Flip { get { return _flip; } }
    public Transform TargetUnitTf { get { return _targetUnit.transform; } }
    public Unit TargetUnit { get { return _targetUnit; } set { _targetUnit = value; } }
    public float VisionRadius { get { return _visionRadius; } }
    public LayerMask WeaponMask { get { return _weaponMask; } }
    public LayerMask GroundMask { get { return _groundMask; } }
    public LayerMask ObstacleMask { get { return _playerMask; } }
    public LayerMask AttackMask { set { _attackMask = value; } }

    public float TimeToNextShoot { get { return _timeToNextShoot; } set { _timeToNextShoot = value; } }
    public float TimeBeforeAction { get { return _timeBeforeAction; } }
    public float TimerBeforeAction { get { return _timerBeforeAction; } set { _timerBeforeAction = value; } }
    public float ShootEvery { get { return _shootEvery; } }
    public float StunTime { get { return _stunTime; } set { _stunTime = value; } }
    public float RunSpeed { get { return _runSpeed; } }
    public float WalkSpeed { get { return _walkSpeed; } }
    public float MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }
    public float LerpAmount { get { return _lerpAmount; } }
    public float Acceleration { get { return _acceleration; } }
    public float Deceleration { get { return _deceleration; } }
    public bool IsGrounded { get { return _isGrounded; } }

    public GameObject StunAnimGO { get { return _stunAnimGO; } }

    public PlayerController UnitController { get { return _unitController; } }
    public WeaponController WeaponController { get { return _weaponController; } }


    private void Awake()
    {
        _weaponController = GetComponent<WeaponController>();
        _unitController = GetComponent<PlayerController>();
        _spriteRenderer = transform.Find("Graphics").GetComponent<SpriteRenderer>();
        _col = GetComponent<CircleCollider2D>();
        _rig = GetComponent<Rigidbody2D>();
        _headTrigger = GetComponentInChildren<HeadTrigger>();

        _flip = GetComponent<Flip>();

        _weaponController.OnWeaponChange += _weaponController_OnWeaponChange;

        _headTrigger.OnPlayerJumpedOnHead += _headTrigger_OnPlayerJumpedOnHead;
        _headTrigger.OnPlayerJumpedOffHead += _headTrigger_OnPlayerJumpedOffHead;
    }

    private void _headTrigger_OnPlayerJumpedOffHead(object sender, Collider2D collision)
    {
        StartCoroutine(StopIgnoringCollisionAfter(_col, collision, .2f));
    }

    private IEnumerator StopIgnoringCollisionAfter(Collider2D col1, Collider2D col2, float time)
    {
        yield return new WaitForSeconds(time);

        Physics2D.IgnoreCollision(col1, col2, false);
    }

    private void _headTrigger_OnPlayerJumpedOnHead(object sender, Collider2D collision)
    {
        StopAllCoroutines();
        Physics2D.IgnoreCollision(_col, collision, true);
    }

    private void _weaponController_OnWeaponChange(object sender, IWeapon iWeapon)
    {
        if (iWeapon == null)
            return;

        _attackRadius = iWeapon.GetAttackDistance();
        iWeapon.SetAttackMask(_attackMask);
    }

    private void Start()
    {
        TimeToNextShoot = Time.time + _shootEvery;

        if(_weaponToTake != null)
            _weaponController.TakeWeapon(_weaponToTake);

        _states = new EnemyStateFactory(this);
        _currentState = _states.Grounded();
        
        _currentState.OnEnter(this);
    }

    private void FixedUpdate()
    {
        if (_isPossessed)
            return;

        _isGrounded = GroundCheck();

        _currentState.UpdateStates(this);

        SearchForNewWeapon();
    }

    private void SearchForNewWeapon()
    {
        if (_weaponController.IsWeaponTaken || _stunTime > 0f)
            return;

        IWeapon iWeapon = _weaponController.CheckForWeapon();
        if (iWeapon != null)
        {
            if (iWeapon.IsFlying())
                return;

            _weaponController.TakeWeapon(iWeapon);
        }
    }

    private bool GroundCheck()
    {
        return Physics2D.CircleCast(transform.position, _col.radius, Vector2.down, _checkDistance, _groundMask);
    }

    public void Possess(LayerMask attackMask)
    {
        _spriteRenderer.sprite = _possesedStateSprite;

        gameObject.layer = 8;

        _unitController.enabled = true;

        _stunAnimGO.SetActive(false);

        _isPossessed = true;

        _attackMask = attackMask;
        _weaponController.SetAttackMask(attackMask);
    }

    public void UnPossess(LayerMask attackMask, Transform targetUnit)
    {
        _spriteRenderer.sprite = _normalStateSprite ;

        gameObject.layer = 13;

        _unitController.enabled = false;

        _stunAnimGO.SetActive(true);

        _isPossessed = false;

        _attackMask = attackMask;
        _weaponController.SetAttackMask(attackMask);

        //Debug.Log(GroundCheck());
        if (!GroundCheck()) {

            Vector2 dir = (transform.position - targetUnit.position).normalized;
            _rig.velocity += dir * _unpossessFlyPower;
        }
    }

    public bool CanISeeMyTarget()
    {
        Vector2 dir = (_targetUnit.transform.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, float.PositiveInfinity, _groundMask + _playerMask);
        if (hit.transform == _targetUnit.transform)
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _visionRadius);*/
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }

    public void Stun(float time)
    {
        _stunTime = time;

        _currentState.UpdateStates(this);

        /*_currentState = _states.Stun();

        _currentState.OnEnter(this);*/
    }
}
