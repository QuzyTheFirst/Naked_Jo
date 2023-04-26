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
        if (Time.time < _lastAttackTime + _weaponParams.FireRate)
            return false;

        if (CurrentAmmo <= 0)
        {
            Debug.Log("No Ammo");
            return false;
        }

        float distanceFromPlayer = 1.25f;
        Vector2 dir = (target.position - UnitController.transform.position).normalized;
        Vector2 spawnPos = (Vector2)UnitController.transform.position + (dir * distanceFromPlayer);

        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Transform bulletTf = SpawnBullet(spawnPos, dir, distanceFromPlayer, _weaponParams.BulletFlyingMask, Quaternion.Euler(0, 0, rotZ));

        if (bulletTf != null)
        {
            Bullet bullet = bulletTf.GetComponent<Bullet>();

            float bulletPower = 20f;
            bullet.Rig.velocity = dir * bulletPower;
            bullet.ShootPos = transform.position;
        }

        SoundManager.Instance.Play("PistolShoot"); 

        CurrentAmmo--;
        _lastAttackTime = Time.time;
        return true;
    }

    public override bool AIShoot(Unit targetUnit)
    {
        if (Time.time < _lastAttackTime + _weaponParams.FireRate)
            return false;

        if (CurrentAmmo <= 0)
        {
            Debug.Log("No Ammo");
            return false;
        }

        float distanceFromPlayer = 1.25f;
        Vector2 dir = (targetUnit.transform.position - UnitController.transform.position).normalized;
        Vector2 spawnPos = (Vector2)UnitController.transform.position + (dir * distanceFromPlayer);

        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Transform bulletTf = SpawnBullet(spawnPos, dir, distanceFromPlayer, _weaponParams.BulletFlyingMask, Quaternion.Euler(0, 0, rotZ));

        if (bulletTf != null)
        {
            Bullet bullet = bulletTf.GetComponent<Bullet>();

            float bulletPower = 20f;
            bullet.Rig.velocity = dir * bulletPower;
            bullet.ShootPos = transform.position;
        }

        SoundManager.Instance.Play("PistolShoot");

        CurrentAmmo--;
        _lastAttackTime = Time.time;
        return true;
    }

    protected Transform SpawnBullet(Vector2 spawnPos, Vector3 dir, float distanceFromPlayer, LayerMask mask, Quaternion rotation)
    {
        RaycastHit2D hit = Physics2D.Raycast(UnitController.transform.position, dir, distanceFromPlayer, mask);
        if(hit.transform != null)
        {
            IDamagable iDamagable = hit.transform.GetComponent<IDamagable>();
            if (iDamagable != null)
            {
                iDamagable.Damage(0f);
            }

            Rigidbody2D rig = hit.transform.GetComponent<Rigidbody2D>();
            if (rig != null)
            {
                Vector2 flyDir = (hit.transform.position - UnitController.transform.position).normalized;
                rig.velocity = flyDir * 6;
            }
        }
        else
        {
            return Instantiate(_weaponParams.BulletPf, spawnPos, rotation);
        }

        return null;
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
