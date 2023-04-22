using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : PlayerComponentGetter
{
    // Movement
    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 10;
    private float _maxSpeedModifier = 1;
    [SerializeField] private float _acceleration = 8;
    [SerializeField] private float _decceleration = 10;
    [SerializeField] private float _lerpAmount = 8;
    [SerializeField] private float _accelInAir = 7;
    [SerializeField] private float _deccelInAir = 7;

    [SerializeField] private bool _doConserveMomentum;

    private bool _isLookingRight = true;

    private Vector2 _direction;
    private Vector2 _velocity;

    // Jump
    [Header("Jump")]
    [SerializeField] private float _jumpHeight = 5;
    private bool _isJumpButtonPressed;

    [SerializeField] private float _jumpPressedRememberTime = .2f;
    private float _jumpPressedRemember;

    [SerializeField, Range(0, 5)] private int _maxAirJumps = 0;
    [SerializeField, Range(0f, 5f)] private float _downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float _upwardMovementMultiplier = 1.7f;

    [SerializeField] private int _jumpPhase;
    private float _defaultGravityScale = 1f;

    // Attack
    private int _attacksSinceLastGrounded;

    // Ground Check
    [Header("Ground Check")]

    [SerializeField] private Transform _groundCheck;

    [SerializeField] private float _groundCheckDistance = .2f;
    [SerializeField] private float _groundRememberTime = .2f;

    [SerializeField, Range(0, 90)] private float _maxGroundAngle = 50f;

    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private PhysicsMaterial2D _frictionLessMat;
    [SerializeField] private PhysicsMaterial2D _fullFrictionMat;

    private Vector2 _groundCheckOffset;

    private int _stepsSinceLastGrounded;
    private int _stepsSinceLastJump;

    private bool _isGrounded;

    private float _groundRemember;
    private float _minGroundDotProduct;

    private float _stopLookingForGroundSteps;

    // Input
    private bool _isSlowMotionButtonPressed = false;
    private float _possessionRadius = 4f;

    // Snap to Ground
    private float _maxSnapSpeed;

    // Player States
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;

    // Rolling
    [Header("Rolling")]
    [SerializeField] private float _rollingDistance;
    [SerializeField] private float _rollingSpeed;
    [SerializeField] private LayerMask _rollingObstacles;
    private bool _doRoll = false;
    private float _rollingDirection;

    // Look For Ground
    public float StopLookingForGroundSteps { get { return _stopLookingForGroundSteps; } set { _stopLookingForGroundSteps = value; } }

    // Main Components
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerStateFactory States { get { return _states; } }
    public Rigidbody2D RigidBody { get { return _rig; } }
    public SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }
    public CircleCollider2D Col { get { return _col; } }

    // Walking
    public float WalkDirection { get { return _direction.x; } set { _direction.x = value; } }
    public bool IsWalking { get { return _direction.x != 0; } }
    public float MaxSpeed { get { return _maxSpeed; } }
    public float MaxSpeedModifier { get { return _maxSpeedModifier; } set { _maxSpeedModifier = value; } }
    public float LerpAmount { get { return _lerpAmount; } }
    public float Acceleration { get { return _acceleration; } }
    public float Deceleration { get { return _decceleration; } }
    public float AccelInAir { get { return _accelInAir; } }
    public float DeccelInAir { get { return _deccelInAir; } }
    public bool DoConserveMomentum { get { return _doConserveMomentum; } }
    public Vector2 Velocity { get { return _velocity; } set { _velocity = value; } }

    // Ground
    public float GroundRemember { get { return _groundRemember; } set { _groundRemember = value; } }
    public Transform GroundCheckTf { get { return _groundCheck; } }
    public PhysicsMaterial2D FullFrictionMat { get { return _fullFrictionMat; } }
    public PhysicsMaterial2D FrictionLessMat { get { return _frictionLessMat; } }

    // Jump
    public float JumpPressedRemember { get { return _jumpPressedRemember; } set { _jumpPressedRemember = value; } }
    public float JumpHeight { get { return _jumpHeight; } }
    public float UpwardMovementMultiplier { get { return _upwardMovementMultiplier; } }
    public float DownwardMovementMultiplier { get { return _downwardMovementMultiplier; } }
    public float DefaultGravityScale { get { return _defaultGravityScale; } }
    public int StepsSinceLastJump { get { return _stepsSinceLastJump; } set { _stepsSinceLastJump = value; } }
    public bool IsJumpButtonPressed { get { return _isJumpButtonPressed; } }
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }

    public int MaxAirJumps { get { return _maxAirJumps; } }
    public int JumpPhase { get { return _jumpPhase; } set { _jumpPhase = value; } }

    // Weapon
    public WeaponController WeaponController { get { return _weaponController; } }

    // Roll
    public bool DoRoll { get { return _doRoll; } set { _doRoll = value; } }
    public float RollingDirection { get { return _rollingDirection; } set { _rollingDirection = value; } }
    public float RollingDistance { get { return _rollingDistance; }}
    public float RollingSpeed { get { return _rollingSpeed; } }
    public LayerMask RollingObstacles { get { return _rollingObstacles; } }

    private void OnValidate()
    {
        _minGroundDotProduct = Mathf.Cos(_maxGroundAngle * Mathf.Deg2Rad);
    }

    private new void Awake()
    {
        base.Awake();

        //States
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();

        _currentState.OnEnter(this);
    }

    private void FixedUpdate()
    {
        if (_isGrounded)
        {
            _groundRemember = _groundRememberTime;
            _stepsSinceLastGrounded = 0;
            _jumpPhase = 0;
        }
        else
        {
            _stepsSinceLastGrounded++;
            _groundRemember -= Time.fixedDeltaTime;
        }

        _velocity = _rig.velocity;

        _currentState.UpdateStates(this);

        _rig.velocity = _velocity;

        _jumpPressedRemember -= Time.fixedDeltaTime;

        _isGrounded = GroundCheck();

        //Debug.Log($"Current State: {_currentState} | Current Sub State: {_currentState.GetSubState()} | Direction: {_direction.x}");
    }

    private bool GroundCheck()
    {
        return Physics2D.CircleCast(transform.position, _col.radius, Vector2.down, _groundCheckDistance, _groundMask);
    }

    // Inputs Yes
    public void MovePerformed(float value)
    {
        _direction.x = value;

        _flip.TryToFlip(value);
    }

    public void MoveCanceled()
    {
        _direction.x = 0f;
    }

    public void JumpPerformed()
    {
        _jumpPressedRemember = _jumpPressedRememberTime;
        _isJumpButtonPressed = true;
    }

    public void JumpCanceled()
    {
        _isJumpButtonPressed = false;
    }
    //------------------------
}

