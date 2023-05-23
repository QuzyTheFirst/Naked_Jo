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
        float distanceFromPlayer = _weaponParams.WeaponDistanceFromUnit;
        MainTf.localPosition = dir * distanceFromPlayer;
    }

    public override bool Shoot(Transform target)
    {
        if (Time.time < _lastAttackTime + _weaponParams.PlayerAttackRate)
            return false;

        if (_currentAmmo <= 0)
        {
            Debug.Log("No Ammo");
            return false;
        }

        Transform unitTf = UnitController.transform;

        float distanceFromPlayer = 1.25f;
        Vector2 vectorToTarget = (target.position - unitTf.position);
        Vector2 perpendicular = Vector2.Perpendicular(vectorToTarget);

        for (int i = 0; i < _weaponParams.AmountOfBulletsToSpawn; i++)
        {
            float randNum = Random.Range(_weaponParams.BulletSpreadAmount, -_weaponParams.BulletSpreadAmount);

            Vector2 dir = (vectorToTarget + perpendicular * randNum).normalized;

            Vector2 spawnPos = (Vector2)unitTf.transform.position + (dir * distanceFromPlayer);

            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Transform bulletTf = SpawnBullet(unitTf, _weaponParams, spawnPos, dir, distanceFromPlayer, _weaponParams.BulletFlyingMask, Quaternion.Euler(0, 0, rotZ));

            if (bulletTf != null)
            {
                Bullet bullet = bulletTf.GetComponent<Bullet>();

                float bulletPower = 20f;
                bullet.Rig.velocity = dir * bulletPower;
                bullet.ShootPos = unitTf.position;
            }
        }

        SoundManager.Instance.Play("PistolShoot");

        _currentAmmo--;
        _lastAttackTime = Time.time;
        return true;
    }

    public override bool AIShoot(Unit targetUnit)
    {
        if (Time.time < _lastAttackTime + _weaponParams.PlayerAttackRate)
            return false;

        if (_currentAmmo <= 0)
        {
            Debug.Log("No Ammo");
            return false;
        }

        Transform unitTf = UnitController.transform;

        float distanceFromPlayer = 1.25f;
        Vector2 vectorToTarget = targetUnit.transform.position - unitTf.position;
        Vector2 perpendicular = Vector2.Perpendicular(vectorToTarget);

        for (int i = 0; i < _weaponParams.AmountOfBulletsToSpawn; i++)
        {
            float randNum = Random.Range(_weaponParams.BulletSpreadAmount, -_weaponParams.BulletSpreadAmount);

            Vector2 dir = (vectorToTarget + perpendicular * randNum).normalized;

            Vector2 spawnPos = (Vector2)unitTf.transform.position + (dir * distanceFromPlayer);

            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Transform bulletTf = SpawnBullet(unitTf, _weaponParams, spawnPos, dir, distanceFromPlayer, _weaponParams.BulletFlyingMask, Quaternion.Euler(0, 0, rotZ));

            if (bulletTf != null)
            {
                Bullet bullet = bulletTf.GetComponent<Bullet>();

                float bulletPower = 20f;
                bullet.Rig.velocity = dir * bulletPower;
                bullet.ShootPos = unitTf.position;
            }
        }

        SoundManager.Instance.Play("PistolShoot");

        _currentAmmo--;
        _lastAttackTime = Time.time;
        return true;
    }

    private Transform SpawnBullet(Transform unitTf, RangeWeaponParams weaponParams, Vector2 spawnPos, Vector3 dir, float distanceFromPlayer, LayerMask mask, Quaternion rotation)
    {
        RaycastHit2D hit = Physics2D.Raycast(unitTf.position, dir, distanceFromPlayer, mask);
        if (hit.transform != null)
        {
            IDamagable iDamagable = hit.transform.GetComponent<IDamagable>();
            if (iDamagable != null)
            {
                iDamagable.Damage(1);
            }

            Rigidbody2D rig = hit.transform.GetComponent<Rigidbody2D>();
            if (rig != null)
            {
                Vector2 flyDir = (hit.transform.position - unitTf.position).normalized;
                rig.velocity = flyDir * 6;
            }
        }
        else
        {
            return Instantiate(weaponParams.BulletPf, spawnPos, rotation);
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

    public override WeaponParams GetWeaponParams()
    {
        return _weaponParams;
    }

    public override void ResetAmmo()
    {
        _currentAmmo = _weaponParams.Ammo;
    }

    public override bool IsEmpty()
    {
        return _currentAmmo <= 0;
    }
}
