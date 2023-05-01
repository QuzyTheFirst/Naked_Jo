using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit
{
    private AIBase _enemy;

    public EnemyUnit(AIBase enemy)
    {
        _enemy = enemy;
    }

    public void Stun(float time)
    {
        _enemy.Stun(time);
    }

    public void SetTargetUnit(Unit target)
    {
        _enemy.SetTargetUnit(target, .5f);
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
        return _enemy.MySpriteRenderer;
    }
}
