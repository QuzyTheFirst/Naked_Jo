using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParams : ScriptableObject
{
    [Header("Main")]
    public float AttackRate;
    public float DistanceFromPlayer = .5f;

    [Header("AI")]
    public float DistanceBeforeAttack;
}
