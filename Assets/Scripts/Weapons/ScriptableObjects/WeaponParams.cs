using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParams : ScriptableObject
{
    [Header("Main")]
    public float PlayerAttackRate;
    public float WeaponDistanceFromUnit = .5f;

    [Header("AI")]
    public float EnemyAttackRate = 1f;
    public float DistanceBeforeInitiatingAttack;
}
