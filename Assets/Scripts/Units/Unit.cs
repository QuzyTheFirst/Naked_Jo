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

    protected PlayerController PlayerController { get { return _playerController; }}
    protected SimpleEnemy SimpleEnemy { get { return _enemyController; } }

    protected SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }

    protected Flip Flip { get { return _flip; } }


    private void Awake()
    {
        base.GetAllComponents(true);

        Player = new PlayerUnit(PlayerController);
        Enemy = new EnemyUnit(SimpleEnemy);
    }

    public bool IsPlayer { get { return _isPlayer; } }
    public Collider2D Col { get { return _col; } }
    public WeaponController WeaponController { get { return _weaponController; } }
    public KeyHolder KeyHolder { get { return _keyHolder; } }

    public void Damage(float amount)
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnter?.Invoke(this, collision);
    }
}
