using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolShoot : MonoBehaviour
{
    public static bool Shoot(Transform targetTf, Transform unitTf, ref float lastAttackTime, ref int currentAmmo, RangeWeaponParams weaponParams)
    {
        if (Time.time < lastAttackTime + weaponParams.AttackRate)
            return false;

        if (currentAmmo <= 0)
        {
            Debug.Log("No Ammo");
            return false;
        }

        float distanceFromPlayer = 1.25f;
        Vector2 dir = (targetTf.position - unitTf.position).normalized;
        Vector2 spawnPos = (Vector2)unitTf.position + (dir * distanceFromPlayer);

        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Transform bulletTf = SpawnBullet(unitTf, weaponParams, spawnPos, dir, distanceFromPlayer, weaponParams.BulletFlyingMask, Quaternion.Euler(0, 0, rotZ));

        if (bulletTf != null)
        {
            Bullet bullet = bulletTf.GetComponent<Bullet>();

            float bulletPower = 20f;
            bullet.Rig.velocity = dir * bulletPower;
            bullet.ShootPos = unitTf.position;
        }

        SoundManager.Instance.Play("PistolShoot");

        currentAmmo--;
        lastAttackTime = Time.time;
        return true;
    }

    public static bool AIShoot(Unit targetUnit, Transform unitTf, ref float lastAttackTime, ref int currentAmmo, RangeWeaponParams weaponParams)
    {
        if (Time.time < lastAttackTime + weaponParams.AttackRate)
            return false;

        if (currentAmmo <= 0)
        {
            Debug.Log("No Ammo");
            return false;
        }

        float distanceFromPlayer = 1.25f;
        Vector2 dir = (targetUnit.transform.position - unitTf.transform.position).normalized;
        Vector2 spawnPos = (Vector2)unitTf.transform.position + (dir * distanceFromPlayer);

        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Transform bulletTf = SpawnBullet(unitTf, weaponParams, spawnPos, dir, distanceFromPlayer, weaponParams.BulletFlyingMask, Quaternion.Euler(0, 0, rotZ));

        if (bulletTf != null)
        {
            Bullet bullet = bulletTf.GetComponent<Bullet>();

            float bulletPower = 20f;
            bullet.Rig.velocity = dir * bulletPower;
            bullet.ShootPos = unitTf.position;
        }

        SoundManager.Instance.Play("PistolShoot");

        currentAmmo--;
        lastAttackTime = Time.time;
        return true;
    }

    private static Transform SpawnBullet(Transform unitTf, RangeWeaponParams weaponParams, Vector2 spawnPos, Vector3 dir, float distanceFromPlayer, LayerMask mask, Quaternion rotation)
    {
        RaycastHit2D hit = Physics2D.Raycast(unitTf.position, dir, distanceFromPlayer, mask);
        if (hit.transform != null)
        {
            IDamagable iDamagable = hit.transform.GetComponent<IDamagable>();
            if (iDamagable != null)
            {
                iDamagable.Damage(0f);
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
}
