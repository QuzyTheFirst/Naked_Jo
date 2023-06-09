using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : ComponentsGetter, IDamagable
{
    [SerializeField] protected bool _isPlayer = false;
    [SerializeField] private Sprite _deadBody;
    [SerializeField] private PhysicsMaterial2D _deadBodyMat;

    private bool _isDead = false;
    private bool _hasExploded = false;

    public PlayerUnit Player;
    public EnemyUnit Enemy;

    public static event EventHandler OnDeath;
    public static event EventHandler<Collision2D> OnCollisionEnter;

    public bool IsPlayer { get { return _isPlayer; } }
    public bool IsDead { get { return _isDead; } }
    public bool HasExploded { get { return _hasExploded; } set { _hasExploded = value; } }

    public bool IsAttacking { 
        get {
            if (MyWeaponController != null)
                return MyWeaponController.IsAttacking;

            return false;
        }
    }

    private void Awake()
    {
        base.GetAllComponents(true);

        Player = new PlayerUnit(MyPlayerController);
        Enemy = new EnemyUnit(MyEnemyController);
    }

    public void KillUnit()
    {
        if (_isDead)
            return;

        if(_deadBody != null)
            MySpriteRenderer.sprite = _deadBody;

        gameObject.layer = 12;
        MyRigidbody.sharedMaterial = _deadBodyMat;
        MyCircleCollider.sharedMaterial = _deadBodyMat;
        MySpriteRenderer.sortingOrder = -1;
        MySpriteRenderer.color = Color.grey;

        Player.MoveCanceled();
        Player.JumpCanceled();

        Destroy(MyGroundChecker.gameObject);
        Destroy(MyPlayerController.gameObject);

        if (!_isPlayer)
        {
            MyEnemyController.StunAnimGO.SetActive(false);
            MyEnemyController.Movement = AIBase.MovementState.Stop;

            MyWeaponController.DropWeapon();

            Destroy(MyEnemyController.gameObject);
            Destroy(MyWeaponController.gameObject);
            Destroy(MyHeadTrigger.gameObject);
        }

        _isDead = true;

        enabled = false;
    }

    public void Damage(int amount)
    {
        if (IsPlayer)
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            if (MyEnemyController.Damage(amount))
                OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnter?.Invoke(this, collision);
    }
}
