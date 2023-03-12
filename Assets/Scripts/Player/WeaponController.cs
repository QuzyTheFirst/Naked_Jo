using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private bool _canPickUpWeapons = true;
    [SerializeField] private float _searchRange = .8f;
    [SerializeField] private LayerMask _weaponLayerMask = 11;

    [Header("Unremovable Weapon")]
    [SerializeField] private GameObject _unremovableWeapon;
    private IWeapon _unremovableIWeapon;

    private PlayerController _unitController;
    private IWeapon _currentWeapon;

    private Vector2 _targetPos = default;

    public event EventHandler<IWeapon> OnWeaponChange;

    public Vector2 TargetPos { get { return _targetPos; } set { _targetPos = value; } }
    public bool IsWeaponTaken { get { return _currentWeapon != _unremovableIWeapon; } }

    private void Awake()
    {
        _unitController = GetComponent<PlayerController>();

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

    public void Shoot(Transform target)
    {
        if (_currentWeapon == null)
            return;

        _currentWeapon.Shoot(target);
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

        _currentWeapon.ThrowWeapon(_targetPos);

        ActivateUnremovableWeapon();
    }

    private void ActivateUnremovableWeapon()
    {
        if (_unremovableWeapon != null)
        {
            _unremovableWeapon.SetActive(true);
            _currentWeapon = _unremovableIWeapon;
            _currentWeapon.Init(_unitController, transform);
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
            Debug.Log(hit.transform.name);

            IWeapon iWeapon = hit.transform.GetComponent<IWeapon>();

            if (iWeapon != null && iWeapon != _currentWeapon)
            {
                return iWeapon;
            }
        }

        return null;
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

        if (iWeapon != null)
        {
            if (_unremovableWeapon != null)
                _unremovableWeapon.SetActive(false);

            weapon.gameObject.SetActive(true);
            _currentWeapon = iWeapon;
            _currentWeapon.Init(_unitController, transform);
        }

        OnWeaponChange?.Invoke(this, _currentWeapon);
    }

    public void TakeWeapon(IWeapon iWeapon)
    {
        if (!_canPickUpWeapons)
            return;

        DropWeapon();

        if (iWeapon != null)
        {
            if (_unremovableWeapon != null)
                _unremovableWeapon.SetActive(false);

            _currentWeapon = iWeapon;
            _currentWeapon.Init(_unitController, transform);
        }

        OnWeaponChange?.Invoke(this, _currentWeapon);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _searchRange);
    }
}
