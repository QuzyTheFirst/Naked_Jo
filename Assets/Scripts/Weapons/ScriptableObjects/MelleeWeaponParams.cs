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

    public string AttackSoundName;

    public float DistanceFromPlayer = .5f;

    [Header("AI")]
    public float DistanceBeforeAttack;
}
