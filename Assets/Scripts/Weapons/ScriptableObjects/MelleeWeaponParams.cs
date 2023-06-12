using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Parameters", menuName = "Weapons/Mellee Weapon Params")]
public class MelleeWeaponParams : WeaponParams
{
    [Header("Mellee Weapon")]
    public float AttackRange;

    public float AttackDistance;

    public float AttackTime = .1f;

    public float EndAttackTime = .1f;

    public float PrepareTime;

    public string AttackSoundName;

    [Header("Abilities")]
    public bool CanDeflectBullets = false;
}
