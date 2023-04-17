using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public bool Shoot(Transform target);

    //public bool Shoot(Vector2 targetPos);

    public bool AIShoot(Unit targetUnit);

    public void OnUpdate(Vector2 targetPos);

    public void ThrowWeapon(Vector2 targetPos);

    public void DropWeapon();
    public void DropWeapon(Vector2 dir);

    public SpriteRenderer GetSpriteRenderer();

    public bool IsEmpty();

    public bool IsFlying();

    public float GetAttackDistance();
    public float GetFullAttackTime();

    public void SetAttackMask(LayerMask attackMask);

    public void ResetAmmo();

    public void Init(PlayerController sender, Transform parent);

    public void SetParent(Transform parent);

    public void Unparent();
}
