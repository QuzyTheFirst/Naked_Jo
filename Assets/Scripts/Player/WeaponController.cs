using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : ComponentsGetter
{
    [Header("Main")]
    [SerializeField] private bool _canPickUpWeapons = true;
    [SerializeField] private float _searchRange = .8f;
    [SerializeField] private LayerMask _weaponLayerMask = 11;

    [Header("Unremovable Weapon")]
    [SerializeField] private GameObject _unremovableWeapon;
    private IWeapon _unremovableIWeapon;

    private IWeapon _currentWeapon;

    private Vector2 _targetPos = default;

    public event EventHandler<IWeapon> OnWeaponChange;

    private bool _keepAttacking = false;
    private Transform _temporaryTarget;

    public Vector2 TargetPos { get { return _targetPos; } set { _targetPos = value; } }
    public bool IsWeaponTaken { get { return _currentWeapon != _unremovableIWeapon; } }
    public float FullAttackTime 
    { 
        get 
        {
            if(_currentWeapon.GetWeaponType() == Weapon.WeaponType.Mellee)
                return GetMelleeWeaponParams().AttackTime + GetMelleeWeaponParams().PrepareTime;

            return 0;
        } 
    }

    private void Awake()
    {
        base.GetAllComponents(false);

        if(_unremovableWeapon != null)
            _unremovableIWeapon = _unremovableWeapon.transform.GetComponentInChildren<IWeapon>();
    }

    private void Start()
    {
        ActivateUnremovableWeapon();
    }

    private void Update()
    {
        if (_currentWeapon == null)
            return;

        _currentWeapon.OnUpdate(_targetPos);

        if (_keepAttacking && _temporaryTarget != null)
            Shoot(_temporaryTarget);

        UpdateWeaponFlipY();
    }

    private void UpdateWeaponFlipY()
    {
        SpriteRenderer renderer = _currentWeapon.GetSpriteRenderer();

        float dir = Mathf.Sign((_targetPos - (Vector2)transform.position).x);

        if (dir == -1)
            renderer.flipY = true;
        else
            renderer.flipY = false;
    }

    public bool Shoot(Transform target)
    {
        if (_currentWeapon == null)
            return false;

        return _currentWeapon.Shoot(target);
    }

    public bool AIShoot(Unit targetUnit)
    {
        if (_currentWeapon == null)
            return false;

        return _currentWeapon.AIShoot(targetUnit);
    }

    public void DropWeapon()
    {
        if (_currentWeapon == _unremovableIWeapon || _currentWeapon == null)
            return;

        _currentWeapon.DropWeapon();

        ActivateUnremovableWeapon();
    } 

    public void DropWeapon(Vector2 dir)
    {
        if (_currentWeapon == _unremovableIWeapon || _currentWeapon == null)
            return;

        _currentWeapon.DropWeapon(dir);

        ActivateUnremovableWeapon();
    }

    public void ThrowWeapon()
    {
        if (_currentWeapon == _unremovableIWeapon || _currentWeapon == null)
            return;

        SoundManager.Instance.Play("Throw");

        _currentWeapon.ThrowWeapon(_targetPos);

        ActivateUnremovableWeapon();
    }

    private void ActivateUnremovableWeapon()
    {
        if (_unremovableWeapon != null)
        {
            _unremovableWeapon.SetActive(true);
            _currentWeapon = _unremovableIWeapon;
            _currentWeapon.Init(MyUnit, transform);
        }
        else
        {
            _currentWeapon = null;
        }

        OnWeaponChange?.Invoke(this, _currentWeapon);
    }

    public void TryPickUpWeapon()
    {
        if (!_canPickUpWeapons)
            return;

        IWeapon iWeapon = CheckForWeapon();

        ThrowWeapon();

        TakeWeapon(iWeapon);
    }

    public IWeapon CheckForWeapon()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _searchRange, _weaponLayerMask);

        foreach (Collider2D hit in hits)
        {
            //Debug.Log(hit.transform.name);

            IWeapon iWeapon = hit.transform.GetComponent<IWeapon>();

            if (iWeapon != null && iWeapon != _currentWeapon)
            {
                return iWeapon;
            }
        }

        return null;
    }

    public Weapon.WeaponType GetWeaponType()
    {
        if (_currentWeapon == null)
            return Weapon.WeaponType.Mellee;

        return _currentWeapon.GetWeaponType();
    }

    public int GetCurrentAmmo()
    {
        if (_currentWeapon == null)
            return 0;

        return _currentWeapon.GetCurrentAmmo();
    }

    public WeaponParams GetWeaponParams()
    {
        if (_currentWeapon == null)
            return null;

        return _currentWeapon.GetWeaponParams();
    }

    public MelleeWeaponParams GetMelleeWeaponParams()
    {
        return (MelleeWeaponParams)GetWeaponParams();
    }

    public RangeWeaponParams GetRangeWeaponParams()
    {
        return (RangeWeaponParams)GetWeaponParams();
    }

    #region AI

    public void SetAttackMask(LayerMask mask)
    {
        if(_currentWeapon != null)
            _currentWeapon.SetAttackMask(mask);
    }

    public void ResetAmmo()
    {
        if (_currentWeapon == null)
            return;

        _currentWeapon.ResetAmmo();
    }

    public void TakeWeapon(Transform weapon)
    {
        if (!_canPickUpWeapons)
            return;

        IWeapon iWeapon = weapon.GetComponentInChildren<IWeapon>();

        DropWeapon();

        if (iWeapon == null)
            return;

        if (_unremovableWeapon != null)
            _unremovableWeapon.SetActive(false);

        weapon.gameObject.SetActive(true);
        _currentWeapon = iWeapon;
        _currentWeapon.Init(MyUnit, transform);

        OnWeaponChange?.Invoke(this, _currentWeapon);
    }

    public void TakeWeapon(IWeapon iWeapon)
    {
        if (!_canPickUpWeapons)
            return;

        DropWeapon();


        if (iWeapon == null)
            return;

        if (_unremovableWeapon != null)
            _unremovableWeapon.SetActive(false);

        _currentWeapon = iWeapon;
        _currentWeapon.Init(MyUnit, transform);

        OnWeaponChange?.Invoke(this, _currentWeapon);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _searchRange);
    }
}
