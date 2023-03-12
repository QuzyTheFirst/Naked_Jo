using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public void Shoot(Transform target);

    public void OnUpdate(Vector2 targetPos);

    public void ThrowWeapon(Vector2 targetPos);

    public void DropWeapon();
    public void DropWeapon(Vector2 dir);

    public SpriteRenderer GetSpriteRenderer();

    public bool IsEmpty();

    public bool IsFlying();

    public float GetAttackDistance();

    public void SetAttackMask(LayerMask attackMask);

    public void ResetAmmo();

    public void Init(PlayerController sender, Transform parent);

    public void SetParent(Transform parent);

    public void Unparent();
}
