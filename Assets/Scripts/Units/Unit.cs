using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : ComponentsGetter, IDamagable
{
    [SerializeField] protected bool _isPlayer = false;

    public PlayerUnit Player;
    public EnemyUnit Enemy;

    public static event EventHandler OnDeath;
    public static event EventHandler<Collision2D> OnCollisionEnter;

    private void Awake()
    {
        base.GetAllComponents(true);

        Player = new PlayerUnit(MyPlayerController);
        Enemy = new EnemyUnit(MyEnemyController);
    }

    public bool IsPlayer { get { return _isPlayer; } }

    public void Damage(float amount)
    {
        if (IsPlayer)
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            if (MyEnemyController.Damage())
                OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnter?.Invoke(this, collision);
    }
}
