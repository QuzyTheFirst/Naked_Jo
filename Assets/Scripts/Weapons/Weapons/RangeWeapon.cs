using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : Weapon
{
    [SerializeField] private RangeWeaponParams _weaponParams;

    private int _currentAmmo;

    private float _lastAttackTime = 0;

    protected int CurrentAmmo { get { return _currentAmmo; } set { _currentAmmo = value; } }

    private new void Awake()
    {
        base.Awake();
        _currentAmmo = _weaponParams.Ammo;
    }

    public override void Shoot(Vector2 targetPos)
    {
        if (Time.time < _lastAttackTime + _weaponParams.FireRate)
            return;

        if (CurrentAmmo <= 0)
        {
            Debug.Log("No Ammo");
            return;
        }

        float distanceFromPlayer = 1.25f;
        Vector2 dir = (targetPos - (Vector2)UnitController.transform.position).normalized;
        Vector2 spawnPos = (Vector2)UnitController.transform.position + (dir * distanceFromPlayer);

        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Transform bulletTf = SpawnBullet(spawnPos, Quaternion.Euler(0, 0, rotZ));
        Bullet bullet = bulletTf.GetComponent<Bullet>();

        float bulletPower = 20f;
        bullet.Rig.velocity = dir * bulletPower;
        bullet.ShootPos = transform.position;

        CurrentAmmo--;
        _lastAttackTime = Time.time;
    }

    protected Transform SpawnBullet(Vector2 spawnPos, Quaternion rotation)
    {
        return Instantiate(_weaponParams.BulletPf, spawnPos, rotation);
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
