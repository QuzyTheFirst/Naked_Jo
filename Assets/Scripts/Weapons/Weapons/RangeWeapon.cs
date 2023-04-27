using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : Weapon
{
    [SerializeField] private RangeWeaponParams _weaponParams;

    private int _currentAmmo;

    private float _lastAttackTime = 0;

    public int CurrentAmmo { get { return _currentAmmo; } set { _currentAmmo = value; } }

    public float LastAttackTime { get { return _lastAttackTime; } set { _lastAttackTime = value; } }

    public RangeWeaponParams WeaponParams { get { return _weaponParams; } }

    private delegate bool ShootMethod(Transform targetTf, Transform unitTf, ref float lastAttackTime, ref int currentAmmo, RangeWeaponParams weaponParams);

    private delegate bool ShootMethodAI(Unit targetUnit, Transform unitTf, ref float lastAttackTime, ref int currentAmmo, RangeWeaponParams weaponParams);

    ShootMethod _shootMethod;
    ShootMethodAI _shootMethodAI;

    private new void Awake()
    {
        base.Awake();
        _currentAmmo = _weaponParams.Ammo;

        if(_weaponParams.WeaponClass == RangeWeaponParams.WeaponType.Pistol)
        {
            _shootMethod = PistolShoot.Shoot;
            _shootMethodAI = PistolShoot.AIShoot;
        }
    }

    public override void OnUpdate(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)UnitController.transform.position).normalized;

        //Weapon Rotation
        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        MainTf.rotation = Quaternion.Euler(0, 0, rotZ);

        //Weapon Position
        float distanceFromPlayer = _weaponParams.DistanceFromPlayer;
        MainTf.localPosition = dir * distanceFromPlayer;
    }

    public override bool Shoot(Transform target)
    {
        return _shootMethod(target, UnitController.transform, ref _lastAttackTime, ref _currentAmmo, _weaponParams);
    }

    public override bool AIShoot(Unit targetUnit)
    {
        return _shootMethodAI(targetUnit, UnitController.transform, ref _lastAttackTime, ref _currentAmmo, _weaponParams);
    }

    public override WeaponType GetWeaponType()
    {
        return WeaponType.Range;
    }

    public override int GetCurrentAmmo()
    {
        return _currentAmmo;
    }

    public override void ResetAmmo()
    {
        _currentAmmo = _weaponParams.Ammo;
    }

    public override bool IsEmpty()
    {
        return _currentAmmo <= 0;
    }

    public override float GetAttackDistance()
    {
        return _weaponParams.DistanceBeforeAttack;
    }
}
