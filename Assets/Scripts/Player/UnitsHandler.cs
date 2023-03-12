using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UnitsHandler : PlayerInputHandler
{
    [Header("Level")]
    [SerializeField] private GameObject _nextLevelLoader;

    [Header("Cursor")]
    [SerializeField] private CursorController _cursorController;
    [SerializeField] private Color _canPossessColor;
    [SerializeField] private Color _canOnlyUnpossessColor;
    private Vector2 _mousePosInWorld;

    [Header("Possession")]
    [SerializeField] private float _possessionRadius;
    [SerializeField] private float _cooldownTime;
    [SerializeField] private LineRenderer _possessionLineRenderer;
    [SerializeField] private LayerMask _possessionMask;
    [SerializeField] private LayerMask _unpossessionMask;
    [SerializeField] private float _cooldownTimer = 0f;

    [Header("Attack Masks")]
    [SerializeField] private LayerMask _enemyAttackMask;
    [SerializeField] private LayerMask _playerAttackMask;

    [Header("Explosion")]
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _timeToExplode;
    [SerializeField] private float _explosionStunTime;
    [SerializeField] private ParticleSystem _explosionParticles;
    [SerializeField] private Transform _explosionRadiusVisuals;
    private float _explosionTimer;
    private bool _isExplosionGoing;

    // Units
    private List<Unit> _units = null;

    private Unit _playerUnit = null;
    private Unit _currentUnit = null;

    // Camera
    private CameraTargetController _cameraTargetController;
    private KeyHolder _keyHolder;

    // Inputs
    private bool _isSlowMotionButtonPressed = false;
    private bool _isExplosionButtonPressed = false;

    private float _movementDirection = 0f;

    protected override void OnEnable()
    {
        base.OnEnable();

        // Events
        Unit.OnDeath += OnUnitDeath;
        Unit.OnCollisionEnter += Unit_OnCollisionEnter;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        // Events
        Unit.OnDeath -= OnUnitDeath;
        Unit.OnCollisionEnter -= Unit_OnCollisionEnter;
    }

    private new void Awake()
    {
        base.Awake();

        // Read Inputs
        MovementPerformed += UnitsHandler_MovementPerformed;
        MovementCanceled += UnitsHandler_MovementCanceled;

        JumpPerformed += UnitsHandler_JumpPerformed;
        JumpCanceled += UnitsHandler_JumpCanceled;

        AttackPerformed += UnitsHandler_AttackPerformed;

        SlowMotionPerformed += UnitsHandler_SlowMotionPerformed;
        SlowMotionCanceled += UnitsHandler_SlowMotionCanceled;

        PossessPerformed += UnitsHandler_PossessPerformed;

        RestartPerformed += UnitsHandler_RestartPerformed;

        PickUpThrowPerformed += UnitsHandler_PickUpThrowPerformed;

        CrouchPerformed += UnitsHandler_CrouchPerformed;

        ExplodePerformed += UnitsHandler_ExplodePerformed;
        ExplodeCanceled += UnitsHandler_ExplodeCanceled;

        // Components
        _cameraTargetController = GetComponent<CameraTargetController>();
        _keyHolder = GetComponent<KeyHolder>();

        // Find All Units
        _units = new List<Unit>();
        Unit[] units = FindObjectsOfType<Unit>();
        for(int i = 0; i < units.Length; i++)
        {
            _units.Add(units[i]);
        }

        // Find Player Unit
        foreach(Unit unit in _units)
        {
            if (unit.IsPlayer)
            {
                _playerUnit = unit;
                _currentUnit = _playerUnit;
                Debug.Log("Player Setted");
                break;
            }
        }

        CheckForNextLevelLoaderSpawn();
    }

    private void UnitsHandler_ExplodeCanceled(object sender, EventArgs e)
    {
        _isExplosionButtonPressed = false;
    }

    private void UnitsHandler_ExplodePerformed(object sender, EventArgs e)
    {
        _isExplosionButtonPressed = true;
    }

    private void Start()
    {
        // Set Attack Mask
        foreach (Unit unit in _units)
        {
            if (!unit.IsPlayer)
            {
                unit.Enemy.SetAttackMask(_enemyAttackMask);
            }
        }

        SetUnit(_playerUnit);

        if(_nextLevelLoader != null)
            _nextLevelLoader.SetActive(false);
    }

    private void Update()
    {
        Camera mainCamera = Camera.main;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        _mousePosInWorld = mainCamera.ScreenToWorldPoint(mousePos);

        UpdateWeaponTargetPos();

        if (_currentUnit != null)
            _cameraTargetController.UpdateTargetPos(_currentUnit.transform.position);

        CheckForNextLevelLoaderSpawn();

        if (_isSlowMotionButtonPressed)
            ShowPossessionLine();

        CheckForPlayerExplosion();


        _cursorController.SetCursorPosition(_mousePosInWorld);
        if(_cooldownTimer <= 0)
        {
            _cursorController.ChangePossessionIconColor(_canPossessColor);
        }
        else
        {
            _cursorController.ChangePossessionIconColor(_canOnlyUnpossessColor);
        }
        _cursorController.TogglePossessionIcon(_playerUnit != _currentUnit || _cooldownTimer <= 0);


        if (_cooldownTimer >= 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }

    private void CheckForPlayerExplosion()
    {
        if (_isExplosionButtonPressed && !_currentUnit.IsPlayer)
        {
            _isExplosionGoing = true;
            _currentUnit.Player.SetMaxSpeedModifier(.1f);
            _explosionTimer += Time.deltaTime;
            float explosionProcentage = _explosionTimer / _timeToExplode;
            //Debug.Log("Exp proc: " + explosionProcentage);

            SpriteRenderer enemyGraphics = _currentUnit.Enemy.GetGraphics();
            Color newColor = Color.white - Color.white * explosionProcentage;
            enemyGraphics.color = new Color(newColor.r, newColor.g, newColor.b, 255);

            _explosionRadiusVisuals.transform.position = _currentUnit.transform.position;
            //Debug.Log($"Current unit: {_currentUnit.transform.position} | {_currentUnit.transform.name}");
            _explosionRadiusVisuals.gameObject.SetActive(true);

            if (_explosionTimer >= _timeToExplode)
            {
                EnemyUnit enemy = _currentUnit.Enemy;

                _playerUnit.transform.position = _currentUnit.transform.position;

                enemy.UnPossess(_enemyAttackMask, _playerUnit.transform);

                _units.Remove(_currentUnit);
                _currentUnit.Damage(0);

                SetUnit(_playerUnit);

                _playerUnit.gameObject.SetActive(true);

                Collider2D[] colls = Physics2D.OverlapCircleAll(_currentUnit.transform.position, _explosionRadius, _playerAttackMask);
                foreach (Collider2D col in colls)
                {
                    Unit unit = col.GetComponent<Unit>();
                    if (unit != null)
                    {
                        if (unit.IsPlayer)
                            continue;

                        unit.Enemy.Stun(_explosionStunTime);
                    }
                }

                _explosionParticles.transform.position = _playerUnit.transform.position;
                _explosionParticles.Play();

                _explosionTimer = 0;
                _cooldownTimer = 0f;
                _isExplosionGoing = false;

                _explosionRadiusVisuals.gameObject.SetActive(false);
            }
        }
        else
        {
            if (_isExplosionGoing)
            {
                _explosionTimer = 0;

                SpriteRenderer enemyGraphics = _currentUnit.Enemy.GetGraphics();
                enemyGraphics.color = Color.white;
                _currentUnit.Player.SetMaxSpeedModifier(1f);
                _isExplosionGoing = false;

                _explosionRadiusVisuals.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateWeaponTargetPos()
    {
        if (_currentUnit == _playerUnit)
            return;

        _currentUnit.WeaponController.TargetPos = _mousePosInWorld;
    }

    private void CheckForNextLevelLoaderSpawn()
    {
        if (_units.Count == 1 && _nextLevelLoader != null)
        {
            _nextLevelLoader.SetActive(true);
        }
    }

    private void SetUnit(Unit newUnit)
    {
        PlayerUnit player = _currentUnit.Player;
        player.MoveCanceled();
        player.JumpCanceled(); 

        _currentUnit = newUnit;

        _currentUnit.Player.MovePerformed(_movementDirection);

        SetEnemiesTargetUnit();

        //Debug.Log("Unit setted");
    }

    private void SetEnemiesTargetUnit()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            if (_units[i] == _playerUnit)
                continue;

            _units[i].Enemy.SetTargetUnit(_currentUnit.transform);
        }
    }

    private void DropWeapon()
    {
        if (_currentUnit == _playerUnit)
            return;

        _currentUnit.WeaponController.DropWeapon();
    }

    #region Events
    private void Unit_OnCollisionEnter(object sender, Collision2D collision)
    {
        Unit unit = (Unit)sender;

        if(unit == _currentUnit)
        {
            FindAndAddKey(collision);

            FindAndOpenKeyDoor(collision);
        }
    }
    private void FindAndAddKey(Collision2D collision)
    {
        Key key = collision.transform.GetComponent<Key>();
        if (key != null)
        {
            _keyHolder.AddKey(key.GetKeyType());
            GameUIController.Instance.SetKeys(_keyHolder.KeyList);
            Destroy(key.gameObject);
        }
    }
    private void FindAndOpenKeyDoor(Collision2D collision)
    {
        DoorWithKey keyDoor = collision.transform.GetComponent<DoorWithKey>();
        if (keyDoor != null)
        {
            if (_keyHolder.ContainsKey(keyDoor.KeyType))
            {
                _keyHolder.RemoveKey(keyDoor.KeyType);
                GameUIController.Instance.SetKeys(_keyHolder.KeyList);
                keyDoor.OpenDoor(1);
            }
        }
    }


    #region OnDeath
    private void OnUnitDeath(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        if (unit.IsPlayer || _currentUnit == unit)
        {
            if (!_isExplosionGoing) { 
                KillPlayer(unit);
                return;
            }
        }

        KillEnemy(unit);
    }

    private void KillEnemy(Unit unit)
    {
        unit.WeaponController.DropWeapon();

        if (_currentUnit != unit)
        {
            if(unit.KeyHolder != null)
                unit.KeyHolder.DropAllKeys();

            _units.Remove(unit);

            _cooldownTimer = 0f;

            Destroy(unit.transform.gameObject);
            return;
        }

        _playerUnit.transform.position = unit.transform.position;
        unit.Enemy.UnPossess(_enemyAttackMask, _playerUnit.transform);

        SetUnit(_playerUnit);

        _playerUnit.gameObject.SetActive(true);

        if(unit.KeyHolder != null)
            unit.KeyHolder.DropAllKeys();

        _units.Remove(unit);

        _isExplosionGoing = false;
        _explosionRadiusVisuals.gameObject.SetActive(false);
        _explosionTimer = 0f;

        Destroy(unit.transform.gameObject);
    }

    private void KillPlayer(Unit unit)
    {
        PlayerUnit player = unit.Player;

        /*unit.enabled = true;
        unit.WeaponController.DropWeapon();

        Destroy(unit.transform.gameObject);*/

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Debug.Log("You are dead");
    }
    #endregion

    #endregion

    #region Inputs
    private void UnitsHandler_PossessPerformed(object sender, System.EventArgs e)
    {
        if (!_isSlowMotionButtonPressed || _isExplosionButtonPressed)
            return;

        if(_cooldownTimer <= 0)
        {
            if (PossessionRay(_mousePosInWorld))
            {
                _cooldownTimer = _cooldownTime;
            }
            else
            {
                UnpossessionRay(_mousePosInWorld);
            }
        }
        else
        {
            UnpossessionRay(_mousePosInWorld);
        }
    }

    private bool PossessionRay(Vector2 mousePosInWorld)
    {
        RaycastHit2D hit = Physics2D.Raycast(_currentUnit.transform.position, (mousePosInWorld - (Vector2)_currentUnit.transform.position).normalized, _possessionRadius, _possessionMask);

        if (hit.transform != null)
        {
            Unit unit = hit.transform.GetComponent<Unit>();
            if (unit != null)
            {
                if (unit == _currentUnit)
                    return false;

                if (_currentUnit == _playerUnit)
                {
                    _playerUnit.gameObject.SetActive(false);

                    DropWeapon();
                }
                else
                {
                    EnemyUnit currentEnemy = _currentUnit.Enemy;
                    currentEnemy.UnPossess(_enemyAttackMask, unit.transform);
                    currentEnemy.Stun(3f);
                }

                unit.Enemy.Possess(_playerAttackMask);

                _currentUnit = unit;

                TakeAllKeys(_currentUnit);
                SetUnit(_currentUnit);
                return true;
            }
        }
        return false;
    }

    private bool UnpossessionRay(Vector2 mousePosInWorld)
    {
        if (_currentUnit == _playerUnit)
            return false;

        Unit unit = _currentUnit;

        Vector2 unpossessionVector = mousePosInWorld - (Vector2)unit.transform.position;
        float unpossessionDistance = Mathf.Min(_possessionRadius, unpossessionVector.magnitude);

        RaycastHit2D hit = Physics2D.Raycast(_currentUnit.transform.position, unpossessionVector.normalized, _possessionRadius, _unpossessionMask);

        if(hit.transform != null)
        {
            float playerRadius = .5f;
            float rayDistance = hit.distance;

            Debug.Log(hit.transform.name);

            if(rayDistance > 1f)
            {
                _playerUnit.transform.position = (Vector2)unit.transform.position + unpossessionVector.normalized * (rayDistance - playerRadius);
            }
            else
            {
                return false;
            }
        }
        else
        {
            _playerUnit.transform.position = (Vector2)unit.transform.position + unpossessionVector.normalized * unpossessionDistance;
        }

        unit.Enemy.UnPossess(_enemyAttackMask, _playerUnit.transform);
        unit.Enemy.Stun(3f);

        SetUnit(_playerUnit);

        _playerUnit.gameObject.SetActive(true);

        return true;
    }

    private void TakeAllKeys(Unit unit)
    {
        if (unit.KeyHolder == null)
            return;

        foreach(Key.KeyType keyType in unit.KeyHolder.KeyList)
        {
            _keyHolder.AddKey(keyType);
        }

        GameUIController.Instance.SetKeys(_keyHolder.KeyList);
        unit.KeyHolder.ClearKeyList();
    }

    public void UnitsHandler_MovementPerformed(object sender, float value)
    {
        _currentUnit.Player.MovePerformed(value);
        _movementDirection = value;
    }

    public void UnitsHandler_MovementCanceled(object sender, EventArgs e)
    {
        _currentUnit.Player.MoveCanceled();
        _movementDirection = 0f;
    }
    public void UnitsHandler_JumpPerformed(object sender, EventArgs e)
    {
        _currentUnit.Player.JumpPerformed();
    }

    public void UnitsHandler_JumpCanceled(object sender, EventArgs e)
    {
        _currentUnit.Player.JumpCanceled();
    }

    public void UnitsHandler_AttackPerformed(object sender, EventArgs e)
    {
        if (_currentUnit.WeaponController != null)
            _currentUnit.WeaponController.Shoot(_cursorController.transform);
    }

    public void UnitsHandler_SlowMotionPerformed(object sender, EventArgs e)
    {
        _isSlowMotionButtonPressed = true;
        Time.timeScale = .25f;
    }

    public void UnitsHandler_SlowMotionCanceled(object sender, EventArgs e)
    {
        _isSlowMotionButtonPressed = false;
        Time.timeScale = 1f;

        HidePossessionLine();
    }

    private void ShowPossessionLine()
    {
        Vector2 playerPos = _currentUnit.transform.position;
        Vector2 possessionDir = (_mousePosInWorld - playerPos).normalized;

        Vector2 possessionVector = possessionDir * _possessionRadius;

        _possessionLineRenderer.positionCount = 2;
        _possessionLineRenderer.SetPosition(0, playerPos);
        _possessionLineRenderer.SetPosition(1, playerPos + possessionVector);
    }

    private void HidePossessionLine()
    {
        _possessionLineRenderer.positionCount = 0;
    }

    public void UnitsHandler_RestartPerformed(object sender, EventArgs e)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UnitsHandler_PickUpThrowPerformed(object sender, EventArgs e)
    {
        if (_isSlowMotionButtonPressed)
            return;

        if(_currentUnit.WeaponController != null)
            _currentUnit.WeaponController.TryPickUpWeapon();
    }
    public void UnitsHandler_CrouchPerformed(object sender, EventArgs e)
    {
        RaycastHit2D hit = Physics2D.Raycast(_currentUnit.transform.position, Vector2.down, 1f);
        if (hit.transform != null)
        {
            PlatformEffector2D plEf = hit.transform.GetComponent<PlatformEffector2D>();
            if (plEf != null)
            {
                BoxCollider2D collider = hit.transform.GetComponent<BoxCollider2D>();
                Physics2D.IgnoreCollision(collider, _currentUnit.Col, true);
                StartCoroutine(EnableCollision(collider, _currentUnit.Col, .5f));
            }
        }
    }

    private IEnumerator EnableCollision(Collider2D col1, Collider2D col2, float time)
    {
        yield return new WaitForSeconds(time);

        if (col1 != null && col2 != null)
        { 
            Physics2D.IgnoreCollision(col1, col2, false);
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        if (_currentUnit != null)
            Gizmos.DrawWireSphere(_currentUnit.transform.position, _explosionRadius);
        else
            Gizmos.DrawWireSphere(Vector2.zero, _explosionRadius);
    }
}
