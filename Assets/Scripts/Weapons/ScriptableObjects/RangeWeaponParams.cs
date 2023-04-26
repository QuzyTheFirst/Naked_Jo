using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Weapon Parameters", menuName ="Weapons/Range Weapon Params")]
public class RangeWeaponParams : ScriptableObject
{
    [Header("Main")]
    public int Ammo;

    public float FireRate;

    public Transform BulletPf;

    public LayerMask BulletFlyingMask;

    public float DistanceFromPlayer = .5f;

    [Header("AI")]
    public float DistanceBeforeAttack;
}
