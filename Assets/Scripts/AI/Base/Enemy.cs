using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : ComponentsGetter
{
    public enum MovementState
    {
        Left = -1,
        Stop = 0,
        Right = 1,
    }
    private MovementState _movement = MovementState.Right;

    private Unit _targetUnit;

    private bool _isPossessed = false;

    private float _stunTime;

    [Header("Base Enemy")]
    [Header("Movement")]
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _lerpAmount;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;

    private float _movementSpeed;

    [Header("AI")]
    [SerializeField] protected Sprite _possesedStateSprite;
    [SerializeField] protected Sprite _normalStateSprite;
    [SerializeField] protected LayerMask _playerMask;
    [SerializeField] protected float _unpossessFlyPower;

    private bool _canISeeMyTarget;
    private bool _canISeeMyTargetLastFrame;
    private float _attackRadius;

    [SerializeField] private float _timeBeforeAction;
    private float _timerBeforeAction;

    private Vector2 _lastPointWhereTargetWereSeen;

    [Header("Weapon")]
    [SerializeField] private LayerMask _attackMask;

    [Header("Ground")]
    [SerializeField] private LayerMask _groundMask;

    [Header("Stun")]
    [SerializeField] private GameObject _stunAnimGO;

    [Header("Found You")]
    [SerializeField] private GameObject _foundYouGO;

    [Header("Jump State")]
    private bool _doJump = false;
    private bool _jumpedOnHisOwn = false;

    [Header("Fall State")]
    private bool _fallenOnHisOwn;

    // Enemy States
    private EnemyBaseState _currentState;

    // Corutines
    private Coroutine _stopIgnoringCollisionAfterCoroutine;
    private Coroutine _setTargetUnitCoroutine;

    public LayerMask AttackMask { set { _attackMask = value; } }

    public EnemyBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public float MovementDirection { get { return (int)_movement; } }
    public float AttackRadius { get { return _attackRadius; } }
    public MovementState Movement { get { return _movement; } set { _movement = value; } }

    public SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }
    public Rigidbody2D RigidBody { get { return _rig; } }
    public Flip Flip { get { return _flip; } }
    public Transform TargetUnitTf { get { return _targetUnit.transform; } }
    public Unit TargetUnit { get { return _targetUnit; } }

    public LayerMask GroundMask { get { return _groundMask; } }

    public float TimeBeforeAction { get { return _timeBeforeAction; } }
    public float TimerBeforeAction { get { return _timerBeforeAction; } set { _timerBeforeAction = value; } }

    public float StunTime { get { return _stunTime; } set { _stunTime = value; } }
    public float RunSpeed { get { return _runSpeed; } }
    public float WalkSpeed { get { return _walkSpeed; } }
    public float MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }
    public float LerpAmount { get { return _lerpAmount; } }
    public float Acceleration { get { return _acceleration; } }
    public float Deceleration { get { return _deceleration; } }

    public bool IsPossessed { get { return _isPossessed; } }
    public bool IsGrounded { get { return _groundChecker.IsGrounded; } }

    public bool CanISeeMyTarget { get { return _canISeeMyTarget; } }

    //Stun
    public GameObject StunAnimGO { get { return _stunAnimGO; } }

    //Found You
    public GameObject FoundYouGO { get { return _foundYouGO; } }

    public PlayerController PlayerController { get { return _playerController; } }
    public WeaponController WeaponController { get { return _weaponController; } }

    public Vector2 LastPointWhereTargetWereSeen { get { return _lastPointWhereTargetWereSeen; } set { _lastPointWhereTargetWereSeen = value; } }

    //Jump State
    public bool DoJump { get { return _doJump; } set { _doJump = value; } }
    public bool JumpedOnHisOwn { get { return _jumpedOnHisOwn; } set { _jumpedOnHisOwn = value; } }

    //Falling State
    public bool FallenOnHisOwn { get { return _fallenOnHisOwn; } set { _fallenOnHisOwn = value; } }

    protected void Awake()
    {
        GetAllComponents(false);

        _weaponController.OnWeaponChange += _weaponController_OnWeaponChange;

        _headTrigger.OnPlayerJumpedOnHead += _headTrigger_OnPlayerJumpedOnHead;
        _headTrigger.OnPlayerJumpedOffHead += _headTrigger_OnPlayerJumpedOffHead;

        _foundYouGO.SetActive(false);

        _playerController.enabled = false;

        _isPossessed = false;
    }

    protected virtual void FixedUpdate()
    {
        if (_isPossessed)
            return;

        _canISeeMyTargetLastFrame = _canISeeMyTarget;
        _canISeeMyTarget = CanISeeMyTargetMethod();

        if (_canISeeMyTarget)
        {
            _lastPointWhereTargetWereSeen = _targetUnit.transform.position;
        }

        if (_canISeeMyTargetLastFrame == false && _canISeeMyTarget == true && _stunTime <= 0f)
        {
            PlayFoundYouAnim();
        }
    }

    public virtual void Possess(LayerMask attackMask)
    {
        _spriteRenderer.sprite = _possesedStateSprite;

        _col.gameObject.layer = 8;

        _playerController.enabled = true;

        _stunAnimGO.SetActive(false);

        _isPossessed = true;

        _jumpedOnHisOwn = false;
        _fallenOnHisOwn = false;
    }

    public virtual void UnPossess(LayerMask attackMask, Transform targetUnit)
    {
        _spriteRenderer.sprite = _normalStateSprite;

        _col.gameObject.layer = 13;

        _playerController.enabled = false;

        _stunAnimGO.SetActive(true);

        _isPossessed = false;

        //Debug.Log(GroundCheck());
        if (!_groundChecker.IsGrounded)
        {

            Vector2 dir = (transform.position - targetUnit.position).normalized;
            _rig.velocity += dir * _unpossessFlyPower;
        }
    }

    private bool CanISeeMyTargetMethod()
    {
        if (_targetUnit == null)
            return false;

        Vector2 dir = (_targetUnit.transform.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 100f, _groundMask + _playerMask);
        if (hit.transform == _targetUnit.transform)
        {
            return true;
        }

        return false;
    }

    protected void SearchForNewWeapon()
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

    public virtual void Stun(float time)
    {
        _stunTime = time;
    }

    public void SetTargetUnit(Unit newUnit, float time)
    {
        //Debug.Log("New Unit: " + newUnit.transform.name);
        _setTargetUnitCoroutine = StartCoroutine(SetTargetUnitCoroutine(newUnit, time));
    }

    public IEnumerator SetTargetUnitCoroutine(Unit newUnit, float time)
    {
        float timer = 0f;

        while (true)
        {
            if (_targetUnit == null)
            {
                _targetUnit = newUnit;
                break;
            }

            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime;
            if (timer >= time)
            {
                _targetUnit = newUnit;
                break;
            }
        }

        //Debug.Log("Current Unit: " + _targetUnit.transform.name);
    }

    public void PlayFoundYouAnim()
    {
        _foundYouGO.transform.localPosition = Vector2.up * .5f;
        _foundYouGO.SetActive(true);
        LeanTween.moveLocal(_foundYouGO, Vector2.up * .9f, .25f).setEase(LeanTweenType.easeInQuad).setOnComplete(() => { _foundYouGO.SetActive(false); });
    }

    private void _weaponController_OnWeaponChange(object sender, IWeapon iWeapon)
    {
        if (iWeapon == null)
            return;

        _attackRadius = iWeapon.GetWeaponParams().DistanceBeforeInitiatingAttack;
        iWeapon.SetAttackMask(_attackMask);
    }

    private void _headTrigger_OnPlayerJumpedOnHead(object sender, Collider2D collision)
    {
        if (_stopIgnoringCollisionAfterCoroutine != null)
            StopCoroutine(_stopIgnoringCollisionAfterCoroutine);

        Physics2D.IgnoreCollision(_col, collision, true);
    }

    private void _headTrigger_OnPlayerJumpedOffHead(object sender, Collider2D collision)
    {
        _stopIgnoringCollisionAfterCoroutine = StartCoroutine(StopIgnoringCollisionAfter(_col, collision, .2f));
    }

    private IEnumerator StopIgnoringCollisionAfter(Collider2D col1, Collider2D col2, float time)
    {
        yield return new WaitForSeconds(time);

        if (col1 == null || col2 == null)
            yield return null;

        Physics2D.IgnoreCollision(col1, col2, false);
    }

}
