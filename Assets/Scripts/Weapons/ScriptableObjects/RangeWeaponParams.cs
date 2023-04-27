using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Weapon Parameters", menuName ="Weapons/Range Weapon Params")]
public class RangeWeaponParams : WeaponParams
{
    public enum WeaponType
    {
        Pistol,
        Shotgun,
        MachineGun
    }

    [Header("Range Weapon")]
    public WeaponType WeaponClass;

    public int Ammo;

    public bool ShootAutomaticaly = false;

    public Transform BulletPf;

    public LayerMask BulletFlyingMask;
}
