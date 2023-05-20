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
    public void DropWeapon(Vector2 dir, float power, float distanceFromUnit);

    public SpriteRenderer GetSpriteRenderer();

    public bool IsEmpty();

    public bool IsFlying();

    public bool IsAttacking();

    public WeaponParams GetWeaponParams();

    public void SetAttackMask(LayerMask attackMask);

    public Weapon.WeaponType GetWeaponType();
    public int GetCurrentAmmo();
    public void ResetAmmo();

    public void Init(Unit sender, Transform parent);

    public void SetParent(Transform parent);

    public void Unparent();
}
