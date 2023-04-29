using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Weapon Parameters", menuName ="Weapons/Range Weapon Params")]
public class RangeWeaponParams : WeaponParams
{
    [Header("Range Weapon")]
    public int Ammo;

    public int AmountOfBulletsToSpawn = 1;

    public float BulletSpreadAmount;

    public bool IsAutomatic = false;

    public Transform BulletPf;

    public LayerMask BulletFlyingMask;
}
