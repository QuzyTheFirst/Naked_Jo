using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class UnitsHandler : PlayerInputHandler
{
    [Header("Cheat")]
    [SerializeField] private bool _enemiesCantSeeYou;

    [Header("Level")]
    [SerializeField] private NextLevelLoader _nextLevelLoader;

    [Header("Cursor")]
    [SerializeField] private CursorController _cursorController;
    [SerializeField] private Color _canPossessColor;
    [SerializeField] private Color _canOnlyUnpossessColor;
    private Vector2 _mousePosInWorld;

    [Header("Possession")]
    [SerializeField] private float _possessionRadius;
    [SerializeField] private float _possessionCooldownTime;
    [SerializeField] private Transform _nakedJOUnpossession;
    [SerializeField] private LineRenderer _possessionLineRenderer;
    [SerializeField] private Gradient _emptyPossessionGradient;
    [SerializeField] private Gradient _targetPossessionGradient;
    [SerializeField] private LayerMask _possessionMask;
    [SerializeField] private LayerMask _unpossessionMask;
    private float _possessionCooldownTimer = 0f;
    private bool _isPossessionKeyPressed = false;

    [Header("Blood")]
    [SerializeField] private ParticleSystem _bloodParticleSystem;
    [SerializeField] private Transform _enemyBodyParts;

    [Header("Attack Masks")]
    [SerializeField] private LayerMask _enemyAttackMask;
    [SerializeField] private LayerMask _playerAttackMask;

    [Header("Explosion")]
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _timeToExplode;
    [SerializeField] private float _explosionStunTime;
    [SerializeField] private ParticleSystem _explosionParticles;
    [SerializeField] private Transform _explosionRadiusVisuals;
    [SerializeField] private float _explosionCooldownTime;
    private Coroutine _explosionCoroutine;
    private Unit _explodingUnit;
    private float _explosionCooldownTimer;
    private float _explosionTimer;
    private bool _isExplosionGoing;

    [Header("SlowMotion")]
    [SerializeField] private float _slowMotionTime;
    private float _slowMotionTimer;
    private bool _isSlowMotionKeyPressed = false;

    [Header("Roll")]
    [SerializeField] private float _rollingCooldownTime;
    private float _rollingCooldownTimer;

    [Header("Crouch")]
    [SerializeField] private LayerMask _platformMask;

    [Header("Interaction")]
    [SerializeField] private float _interactionRadius;
    [SerializeField] private LayerMask _playerInteractionMask;

    [Header("Post Processing")]
    [SerializeField] private PostProcessVolume _slowMoPPVolume;
    [SerializeField] private PostProcessVolume _killPPVolume;
    private PostProcessingController _postProcessingController;

    // Units
    private List<Unit> _units = null;

    private Unit _playerUnit = null;
    private Unit _currentUnit = null;

    // Camera
    private CameraTargetController _cameraTargetController;
    private KeyHolder _keyHolder;

    // Inputs
    private bool _isExplosionButtonPressed = false;

    private float _movementDirection = 0f;

    private Coroutine _keepAttackingCoroutine;

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
        AttackCanceled += UnitsHandler_AttackCanceled;

        SlowMotionPerformed += UnitsHandler_SlowMotionPerformed;
        SlowMotionCanceled += UnitsHandler_SlowMotionCanceled;

        PossessPerformed += UnitsHandler_PossessPerformed;
        PossessCanceled += UnitsHandler_PossessCanceled;

        RestartPerformed += UnitsHandler_RestartPerformed;

        PickUpThrowPerformed += UnitsHandler_PickUpThrowPerformed;

        CrouchPerformed += UnitsHandler_CrouchPerformed;

        ExplodePerformed += UnitsHandler_ExplodePerformed;
        ExplodeCanceled += UnitsHandler_ExplodeCanceled;

        ZoomInPerformed += UnitsHandler_ZoomInPerformed;
        ZoomOutCanceled += UnitsHandler_ZoomOutCanceled;

        InteractionPerformed += UnitsHandler_InteractionPerformed;
        InteractionCanceled += UnitsHandler_InteractionCanceled;

        OpenPauseMenuPerformed += UnitsHandler_OpenPauseMenuPerformed;

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
                //Debug.Log("Player Setted");
                break;
            }
        }

        _postProcessingController = GetComponent<PostProcessingController>();


        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    private void UnitsHandler_OpenPauseMenuPerformed(object sender, EventArgs e)
    {
        GameUIController.Instance.TogglePauseMenu(!GameUIController.IsPauseMenuOpened);
    }

    private void UnitsHandler_InteractionCanceled(object sender, EventArgs e)
    {
        
    }

    private void UnitsHandler_InteractionPerformed(object sender, EventArgs e)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(_currentUnit.transform.position, _interactionRadius, _playerInteractionMask);
        foreach (Collider2D col in colls)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interaction(this);
            }
        }
    }

    private void UnitsHandler_ZoomOutCanceled(object sender, EventArgs e)
    {
        _cameraTargetController.ToggleTargetPosLerp(false);
    }

    private void UnitsHandler_ZoomInPerformed(object sender, EventArgs e)
    {
        _cameraTargetController.ToggleTargetPosLerp(true);
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

        if ((_units.Count - 1) == 0)
        {
            if (_playerUnit != null && _nextLevelLoader != null)
                GameUIController.Instance.ActivateLevelEndPointer(_playerUnit.transform, _nextLevelLoader.transform);
        }

        _slowMotionTimer = _slowMotionTime;

        _nakedJOUnpossession.gameObject.SetActive(false);

        CheckForNextLevelLoaderActivation();
        _postProcessingController.SetUpPostProcessingController(_slowMoPPVolume, _killPPVolume);

        _playerUnit.MyCostumeChanger.LoadCostume();
    }

    private void Update()
    {
        Camera mainCamera = Camera.main;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        _mousePosInWorld = mainCamera.ScreenToWorldPoint(mousePos);

        UpdateWeaponTargetPos();

        if (_currentUnit != null)
            _cameraTargetController.UpdateTargetPos(_currentUnit.transform.position);

        CheckForNextLevelLoaderActivation();

        if (_isPossessionKeyPressed)
            ShowPossessionLine();

        if (_isSlowMotionKeyPressed)
            ChangeSlowMotionTimer(Time.unscaledDeltaTime, false);

        if (_isExplosionGoing)
        {
            _explosionRadiusVisuals.transform.position = _currentUnit.transform.position;
        }

        _cursorController.SetCursorPosition(_mousePosInWorld);
        if(_possessionCooldownTimer <= 0)
        {
            _cursorController.ChangePossessionIconColor(_canPossessColor);
        }
        else
        {
            _cursorController.ChangePossessionIconColor(_canOnlyUnpossessColor);
        }
        _cursorController.TogglePossessionIcon(_playerUnit != _currentUnit || _possessionCooldownTimer <= 0);


        if (_possessionCooldownTimer >= 0f)
        {
            _possessionCooldownTimer -= Time.deltaTime;
        }
        if(_rollingCooldownTimer >= 0f)
        {
            _rollingCooldownTimer -= Time.deltaTime;
        }
        if(_explosionCooldownTimer >= 0f)
        {
            _explosionCooldownTimer -= Time.deltaTime;
        }

        UpdateUIElements();
    }

    private void UpdateUIElements()
    {
        float possessionValue = Mathf.Clamp01(1 - _possessionCooldownTimer / _possessionCooldownTime);
        
        float rollingValue = Mathf.Clamp01(1 - _rollingCooldownTimer / _rollingCooldownTime);
        
        float explosionValue = Mathf.Clamp01(1 - _explosionCooldownTimer / _explosionCooldownTime);

        float slowMotionValue = Mathf.Clamp01(_slowMotionTimer / _slowMotionTime);

        GameUIController.Instance.SetPossessionValue(possessionValue);
        GameUIController.Instance.SetRollingValue(rollingValue);
        GameUIController.Instance.SetExplosionValue(explosionValue);
        GameUIController.Instance.SetSlowMotionValue(slowMotionValue);
    }

    private void ChangeSlowMotionTimer(float value, bool upOrDown)
    {
        if (GameUIController.IsPauseMenuOpened)
            return;

        if(upOrDown)
            _slowMotionTimer += value;
        else
            _slowMotionTimer -= value;

        _slowMotionTimer = Mathf.Clamp(_slowMotionTimer, 0, _slowMotionTime);

        if(_slowMotionTimer == 0)
        {
            Time.timeScale = 1f;
            _postProcessingController.ToggleSlowMoPostProcessing(false);
        }
    }

    private void UpdateWeaponTargetPos()
    {
        if (_currentUnit == _playerUnit)
            return;

        _currentUnit.MyWeaponController.TargetPos = _mousePosInWorld;
    }

    private void CheckForNextLevelLoaderActivation()
    {
        if (_units.Count == 1 && _nextLevelLoader != null)
        {
            _nextLevelLoader.Activate();
        }
    }

    private void SetUnit(Unit newUnit)
    {
        PlayerUnit player = _currentUnit.Player;
        player.MoveCanceled();
        player.JumpCanceled();

        if (!_currentUnit.IsPlayer)
        {
            CancelAutomaticWeaponAttack();
            _currentUnit.MyWeaponController.OnWeaponChange -= WeaponController_OnWeaponChange;
        }

        bool changeImmediately = false;
        /*if (_currentUnit.IsPlayer)
        {
            changeImmediately = true;
        }*/

        if (!newUnit.IsPlayer)
            newUnit.MyWeaponController.OnWeaponChange += WeaponController_OnWeaponChange;

        _currentUnit = newUnit;

        _currentUnit.Player.MovePerformed(_movementDirection);

        GameUIController.Instance.ToggleRolling(_currentUnit.Player.CanRoll);

        SetUIAmmoAmount(newUnit);

        SetEnemiesTargetUnit(newUnit, changeImmediately);
    }

    private void SetEnemiesTargetUnit(Unit targetUnit, bool immediately = false)
    {
        if (_enemiesCantSeeYou)
            return;

        for (int i = 0; i < _units.Count; i++)
        {
            if (_units[i] == _playerUnit)
                continue;

            _units[i].Enemy.SetTargetUnit(targetUnit, immediately);
        }
    }

    private void DropWeapon()
    {
        if (_currentUnit == _playerUnit)
            return;

        _currentUnit.MyWeaponController.DropWeapon();
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

                Vector2 dir = collision.transform.position - transform.position;
                float sign = Mathf.Sign(dir.x);
                keyDoor.OpenDoor((Door.OpenDoorDirection)sign);
            }
        }
    }

    private void UnitsHandler_ExplodePerformed(object sender, EventArgs e)
    {
        if (_currentUnit.IsDead)
            return;

        _isExplosionButtonPressed = true;

        if (_isExplosionButtonPressed && !_currentUnit.IsPlayer && _explosionCooldownTimer < 0f)
        {
            _isExplosionGoing = true;
            _currentUnit.Player.SetMaxSpeedModifier(.1f);

            LeanTween.color(_currentUnit.MySpriteRenderer.gameObject, Color.black, _timeToExplode - .1f);

            _explosionRadiusVisuals.gameObject.SetActive(true);

            SoundManager.Instance.FadeInVolume("Explode", 1f, .2f);

            _explodingUnit = _currentUnit;

            _explosionCoroutine = StartCoroutine(ExplosionCoroutine(_timeToExplode, _explodingUnit));
        }
    }

    private void UnitsHandler_ExplodeCanceled(object sender, EventArgs e)
    {
        _isExplosionButtonPressed = false;

        if(_explosionCoroutine != null)
            StopCoroutine(_explosionCoroutine);

        _isExplosionGoing = false;

        _explosionRadiusVisuals.gameObject.SetActive(false);

        if (_explodingUnit != null)
        {
            _explodingUnit.Player.SetMaxSpeedModifier(1f);

            LeanTween.cancel(_explodingUnit.MySpriteRenderer.gameObject);
            _explodingUnit.MySpriteRenderer.color = Color.white;
        }
    }

    private IEnumerator ExplosionCoroutine(float time, Unit explodingUnit)
    {
        yield return new WaitForSeconds(time);

        if (_isExplosionGoing && explodingUnit != null)
        {
            Unit oldUnit = explodingUnit;
            EnemyUnit enemy = explodingUnit.Enemy;

            _playerUnit.transform.position = explodingUnit.transform.position;

            enemy.UnPossess(_enemyAttackMask, _playerUnit.transform);

            //_units.Remove(_currentUnit);
            SetUnit(_playerUnit);
            _playerUnit.gameObject.SetActive(true);
            _playerUnit.MyCostumeChanger.SetCostume(CostumeChanger.Costumes.Naked);

            oldUnit.HasExploded = true;
            oldUnit.Damage(oldUnit.transform.position, 100);

            Collider2D[] colls = Physics2D.OverlapCircleAll(explodingUnit.transform.position, _explosionRadius, _playerAttackMask);
            foreach (Collider2D coll in colls)
            {
                Unit unit = coll.GetComponent<Unit>();
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
            _explosionCooldownTimer = _explosionCooldownTime;
            SoundManager.Instance.Play("ExplodeConsequences");
            _possessionCooldownTimer = 0f;
            ChangeSlowMotionTimer(_slowMotionTime * .25f, true);
            _isExplosionGoing = false;

            _explosionRadiusVisuals.gameObject.SetActive(false);
        }

        _isExplosionGoing = false;
    }

    #region OnDeath
    private void OnUnitDeath(object sender, EventArgs e)
    {
        //Debug.Log("On Unit Death");

        Unit unit = sender as Unit;

        if (unit.IsPlayer || _currentUnit == unit)
        {
            //if (!_isExplosionGoing) { 
                KillPlayer(unit);
                return;
            //}
        }

        KillEnemy(unit);

        TryToActivateLevelEndPointer();
        CheckForNowKillYourselfActivation();
    }

    private void TryToActivateLevelEndPointer()
    {
        if ((_units.Count - 1) == 0)
        {
            if (_playerUnit != null && _nextLevelLoader != null)
                GameUIController.Instance.ActivateLevelEndPointer(_playerUnit.transform, _nextLevelLoader.transform);
        }
    }

    private void KillEnemy(Unit unit)
    {
        //Debug.Log(unit.name);

        unit.MyWeaponController.DropWeapon(unit.MyFlip.LookDirection, 1f, .5f);

        if (_currentUnit != unit)
        {
            if(unit.MyKeyHolder != null)
                unit.MyKeyHolder.DropAllKeys();

            _units.Remove(unit);

            _possessionCooldownTimer = 0f;
            _explosionCooldownTimer -= _explosionCooldownTime * .5f;
            ChangeSlowMotionTimer(_slowMotionTime * .25f, true);
            _postProcessingController.PlayKillPostProcessAnim();

            //Debug.Log(unit.transform.name + " : " + unit.HasExploded);

            if (unit.HasExploded)
            {
                Instantiate(_enemyBodyParts, unit.transform.position, Quaternion.identity);
                Destroy(unit.transform.gameObject);
            }
            else
            {
                _bloodParticleSystem.transform.position = unit.transform.position;
                _bloodParticleSystem.Play();

                unit.KillUnit();
            }
            return;
        }

        _playerUnit.transform.position = unit.transform.position;
        unit.Enemy.UnPossess(_enemyAttackMask, _playerUnit.transform);

        SetUnit(_playerUnit);

        _playerUnit.gameObject.SetActive(true);

        if(unit.MyKeyHolder != null)
            unit.MyKeyHolder.DropAllKeys();

        _units.Remove(unit);

        _isExplosionGoing = false;
        _explosionRadiusVisuals.gameObject.SetActive(false);
        _explosionTimer = 0f;
        //SoundManager.Instance.FadeAwayVolume("Explode", .2f);
        SoundManager.Instance.Play("ExplodeConsequences");

        Destroy(unit.transform.gameObject);
    }

    private void KillPlayer(Unit unit)
    {
        if (unit.HasExploded)
        {
            Instantiate(_enemyBodyParts, unit.transform.position, Quaternion.identity);

            unit.KillUnit();

            unit.MySpriteRenderer.sprite = null;
        }
        else
        {
            _bloodParticleSystem.transform.position = unit.transform.position;
            _bloodParticleSystem.Play();

            unit.KillUnit();
        }

        ToggleInterfaceVisibility(false);
        GameUIController.Instance.ToggleRestartPanel(true);

        SetEnemiesTargetUnit(null, true);
        //Debug.Log("You are dead");
    }
    #endregion

    #endregion

    #region Inputs
    private void UnitsHandler_PossessPerformed(object sender, System.EventArgs e)
    {
        if (!_isSlowMotionKeyPressed || _isExplosionButtonPressed)
            return;

        if (_currentUnit.IsDead)
            return;

        _isPossessionKeyPressed = true;
    }

    private void UnitsHandler_PossessCanceled(object sender, EventArgs e)
    {
        if (!_isSlowMotionKeyPressed || _isExplosionButtonPressed)
            return;

        if (_currentUnit.IsDead)
        {
            _isPossessionKeyPressed = false;
            HidePossessionLine();
            return;
        }

        _isPossessionKeyPressed = false;

        HidePossessionLine();

        if (_possessionCooldownTimer <= 0)
        {
            if (PossessionRay(_mousePosInWorld))
            {
                _possessionCooldownTimer = _possessionCooldownTime;
                SoundManager.Instance.Play("Possess");
            }
            else if (UnpossessionRay(_mousePosInWorld))
            {
                SoundManager.Instance.Play("Unpossess");
            }
        }
        else if (UnpossessionRay(_mousePosInWorld))
        {
            SoundManager.Instance.Play("Unpossess");
        }

        CheckForNowKillYourselfActivation();
    }

    private void CheckForNowKillYourselfActivation()
    {
        if (_units.Count == 2 && _currentUnit != _playerUnit)
        {
            GameUIController.Instance.ToggleNowKillYourself(true);
        }
        else
        {
            GameUIController.Instance.ToggleNowKillYourself(false);
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
                    currentEnemy.Stun(3f);
                    currentEnemy.UnPossess(_enemyAttackMask, unit.transform);
                }

                unit.Enemy.Possess(_playerAttackMask);

                SetUIAmmoAmount(unit);

                TakeAllKeys(unit);
                SetUnit(unit);
                return true;
            }
        }
        return false;
    }

    private void SetUIAmmoAmount(Unit unit)
    {
        if (unit.MyWeaponController != null)
        {
            if (unit.MyWeaponController.GetWeaponType() == Weapon.WeaponType.Range)
                GameUIController.Instance.SetAmmoAmount(unit.MyWeaponController.GetCurrentAmmo());
            else
                GameUIController.Instance.DisableAmmoAmount();
        }
        else
            GameUIController.Instance.DisableAmmoAmount();
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

            //Debug.Log(hit.transform.name);

            if(rayDistance > 1.5f)
            {
                //_playerUnit.transform.position = (Vector2)unit.transform.position + unpossessionVector.normalized * (rayDistance - playerRadius);
                _playerUnit.transform.position = (Vector2)unit.transform.position + unpossessionVector.normalized * hit.distance + hit.normal * playerRadius;
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

        unit.Enemy.Stun(3f);
        unit.Enemy.UnPossess(_enemyAttackMask, _playerUnit.transform);

        SetUnit(_playerUnit);
        ChangeJOAppearance(CostumeChanger.Costumes.Naked);

        GameUIController.Instance.DisableAmmoAmount();

        _playerUnit.gameObject.SetActive(true);

        return true;
    }

    private void WeaponController_OnWeaponChange(object sender, IWeapon weapon)
    {
        CancelAutomaticWeaponAttack();

        if (weapon.GetWeaponType() == Weapon.WeaponType.Range)
        {
            GameUIController.Instance.SetAmmoAmount(_currentUnit.MyWeaponController.GetCurrentAmmo());
        }
        else
        {
            //Debug.Log("Disabled Ammo");
            GameUIController.Instance.DisableAmmoAmount();
        }
    }

    private void TakeAllKeys(Unit unit)
    {
        if (unit.MyKeyHolder == null)
            return;

        foreach(Key.KeyType keyType in unit.MyKeyHolder.KeyList)
        {
            _keyHolder.AddKey(keyType);
        }

        GameUIController.Instance.SetKeys(_keyHolder.KeyList);
        unit.MyKeyHolder.ClearKeyList();
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
        if (_currentUnit.MyWeaponController == null)
            return;

        //Debug.Log(_currentUnit.WeaponController.IsAutomatic());
        if (_currentUnit.MyWeaponController.GetWeaponType() == Weapon.WeaponType.Range)
        {
            if (_currentUnit.MyWeaponController.GetRangeWeaponParams().IsAutomatic)
            {
                _keepAttackingCoroutine = StartCoroutine(StartAutomaticAttack());
                return;
            }
        }

        bool attacked;

        attacked = _currentUnit.MyWeaponController.Shoot(_cursorController.transform);

        if (attacked)
        {
            if(_currentUnit.MyWeaponController.GetWeaponType() == Weapon.WeaponType.Range)
            {
                GameUIController.Instance.SetAmmoAmount(_currentUnit.MyWeaponController.GetCurrentAmmo());
            }
        }
    }

    private void UnitsHandler_AttackCanceled(object sender, EventArgs e)
    {
        CancelAutomaticWeaponAttack();
    }

    private IEnumerator StartAutomaticAttack()
    {
        bool attacked;

        while (true)
        {
            attacked = _currentUnit.MyWeaponController.Shoot(_cursorController.transform);

            if (attacked)
            {
                GameUIController.Instance.SetAmmoAmount(_currentUnit.MyWeaponController.GetCurrentAmmo());
            }

            yield return new WaitForSeconds(_currentUnit.MyWeaponController.GetWeaponParams().PlayerAttackRate);
        }
    }

    private void CancelAutomaticWeaponAttack()
    {
        if (_keepAttackingCoroutine != null)
            StopCoroutine(_keepAttackingCoroutine);
    }

    public void UnitsHandler_SlowMotionPerformed(object sender, EventArgs e)
    {
        if (_currentUnit.IsDead)
            return;

        if (GameUIController.IsPauseMenuOpened)
            return;

        _isSlowMotionKeyPressed = true;

        if (_slowMotionTimer > 0f)
        {
            Time.timeScale = .25f;
            _postProcessingController.ToggleSlowMoPostProcessing(true);
        }
    }

    public void UnitsHandler_SlowMotionCanceled(object sender, EventArgs e)
    {
        if (GameUIController.IsPauseMenuOpened)
            return;

        _isSlowMotionKeyPressed = false;
        Time.timeScale = 1f;

        _isPossessionKeyPressed = false;
        HidePossessionLine();

        _postProcessingController.ToggleSlowMoPostProcessing(false);
    }

    private void ShowPossessionLine()
    {
        Vector2 playerPos = _currentUnit.transform.position;
        Vector2 possessionDir = (_mousePosInWorld - playerPos).normalized;

        Vector2 possessionVector = possessionDir * _possessionRadius;

        _possessionLineRenderer.positionCount = 2;
        _possessionLineRenderer.SetPosition(0, playerPos);
        _possessionLineRenderer.SetPosition(1, playerPos + possessionVector);

        RaycastHit2D hit = Physics2D.Raycast(playerPos, possessionDir, _possessionRadius, _possessionMask);

        if (hit.transform != null)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                _possessionLineRenderer.colorGradient = _targetPossessionGradient;

                _nakedJOUnpossession.gameObject.SetActive(false);
            }
            else
            {
                _possessionLineRenderer.colorGradient = _emptyPossessionGradient;

                if (_currentUnit != _playerUnit && hit.distance > 1.5f)
                {
                    _nakedJOUnpossession.gameObject.SetActive(true);
                    //_nakedJOUnpossession.position = playerPos + possessionDir * (hit.distance - .5f);
                    _nakedJOUnpossession.position = playerPos + possessionDir * hit.distance + hit.normal * .5f;
                }
                else
                    _nakedJOUnpossession.gameObject.SetActive(false);
            }
        }
        else
        {
            _possessionLineRenderer.colorGradient = _emptyPossessionGradient;

            if (_currentUnit != _playerUnit)
            {
                _nakedJOUnpossession.gameObject.SetActive(true);
                Vector2 unpossessionVector = _mousePosInWorld - (Vector2)_currentUnit.transform.position;
                float unpossessionDistance = Mathf.Min(_possessionRadius, unpossessionVector.magnitude);
                _nakedJOUnpossession.position = playerPos + possessionDir * unpossessionDistance;
            }
        }
    }

    private void HidePossessionLine()
    {
        _possessionLineRenderer.positionCount = 0;

        _nakedJOUnpossession.gameObject.SetActive(false);
    }

    public void UnitsHandler_RestartPerformed(object sender, EventArgs e)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UnitsHandler_PickUpThrowPerformed(object sender, EventArgs e)
    {
        if (_isSlowMotionKeyPressed)
            return;

        if (_currentUnit.IsDead)
            return;

        if (_currentUnit.MyWeaponController != null)
            _currentUnit.MyWeaponController.TryPickUpWeapon();
    }
    public void UnitsHandler_CrouchPerformed(object sender, EventArgs e)
    {
        if (_currentUnit.IsDead)
            return;

        RaycastHit2D hit = Physics2D.Raycast(_currentUnit.transform.position, Vector2.down, 1f, _platformMask);
        if (hit.transform != null)
        {
            PlatformEffector2D plEf = hit.transform.GetComponent<PlatformEffector2D>();
            if (plEf != null)
            {
                BoxCollider2D collider = hit.transform.GetComponent<BoxCollider2D>();
                Physics2D.IgnoreCollision(collider, _currentUnit.MyCircleCollider, true);
                StartCoroutine(EnableCollision(collider, _currentUnit.MyCircleCollider, .5f));
                //Debug.Log("Going Down Through");
                return;
            }
        }

        if(_movementDirection != 0 && _rollingCooldownTimer < 0f)
        {
            if (StartRolling(_movementDirection))
            {
                _rollingCooldownTimer = _rollingCooldownTime;
            }
        }
    }

    private bool StartRolling(float dir)
    {
        if (_currentUnit.IsDead)
            return false;

        return _currentUnit.Player.Roll(dir);
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

    #region CutsceneController
    public void ToggleCurrentUnitVisibility(bool value)
    {
        _currentUnit.MySpriteRenderer.enabled = value;
    }

    public void SetCameraTarget(Vector2 pos)
    {
        _cameraTargetController.SetCameraTargetPos(pos);
    }

    public void ResetCameraTarget()
    {
        _cameraTargetController.ResetCameraTargetPos();
    }

    public void ToggleInterfaceVisibility(bool value)
    {
        GameUIController.Instance.ToggleInterfaceVisibility(value);
    }
    public bool IsCurrentUnitMoving()
    {
        if (_currentUnit.MyPlayerController.IsWalking)
            return true;

        return false;
    }

    public void ChangeJOAppearance(CostumeChanger.Costumes costume)
    {
        if (!_currentUnit.IsPlayer)
            return;

        _currentUnit.MySpriteRenderer.GetComponent<CostumeChanger>().SetCostume(costume);
    }
    #endregion
}
