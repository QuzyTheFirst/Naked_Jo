using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Parameters", menuName = "Weapons/Mellee Weapon Params")]
public class MelleeWeaponParams : ScriptableObject
{
    [Header("Main")]
    public float AttackRange;

    public float AttackDistance;

    public float AttackRate;

    public float AttackTime;

    public float PrepareTime;

    [Header("AI")]
    public float DistanceBeforeAttack;
}
