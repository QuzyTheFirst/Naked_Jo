using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IDamagable
{
    [SerializeField] protected bool _isPlayer = false;

    public PlayerUnit Player;
    public EnemyUnit Enemy;

    public static event EventHandler OnDeath;
    public static event EventHandler<Collision2D> OnCollisionEnter;

    protected PlayerController PlayerController;
    protected SimpleEnemy SimpleEnemy;
    protected SpriteRenderer SpriteRenderer;
    protected Flip Flip;

    private Collider2D _col;
    private WeaponController _weaponController;
    private KeyHolder _keyHolder;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _weaponController = GetComponent<WeaponController>();
        _keyHolder = GetComponent<KeyHolder>();

        SpriteRenderer = GetComponent<SpriteRenderer>();
        Flip = GetComponent<Flip>();

        PlayerController = GetComponent<PlayerController>();
        SimpleEnemy = GetComponent<SimpleEnemy>();

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
