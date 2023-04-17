using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit
{
    private SimpleEnemy _enemy;

    public EnemyUnit(SimpleEnemy enemy)
    {
        _enemy = enemy;
    }

    public void Stun(float time)
    {
        _enemy.Stun(time);
    }

    public void SetTargetUnit(Unit target)
    {
        _enemy.TargetUnit = target;
    }

    public void Possess(LayerMask attackMask)
    {
        _enemy.Possess(attackMask);
    }

    public void UnPossess(LayerMask attackMask, Transform targetUnit)
    {
        _enemy.UnPossess(attackMask, targetUnit);
    }

    public void SetAttackMask(LayerMask attackMask)
    {
        _enemy.AttackMask = attackMask;
    }

    public SpriteRenderer GetGraphics()
    {
        return _enemy.SpriteRenderer;
    }
}
