using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour, IWeapon
{
    public enum WeaponType
    {
        Mellee,
        Range,
    }

    public enum WeaponState
    {
        Flying,
        Laying,
    }

    [SerializeField] private WeaponWrapper _weaponWrapper;
    [SerializeField] private LayerMask _attackMask;

    private WeaponState _weaponState = WeaponState.Laying;

    private Transform _mainTf;

    private Rigidbody2D _rig;

    private SpriteRenderer _spriteRenderer;

    private Unit _unit;

    protected Transform MainTf { get { return _mainTf; } }
    protected Rigidbody2D Rig { get { return _rig; } }
    protected Unit UnitController { get { return _unit; } }
    protected LayerMask AttackMask { get { return _attackMask; } }

    protected void Awake()
    {
        _mainTf = transform.parent;
        _rig = _mainTf.GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _weaponWrapper.OnCollisionTouch += _weaponWrapper_OnCollisionTouch;
    }

    public virtual void OnUpdate(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)UnitController.transform.position).normalized;

        //Weapon Rotation
        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _mainTf.rotation = Quaternion.Euler(0, 0, rotZ);

        //Weapon Position
        float distanceFromPlayer = .5f;
        _mainTf.localPosition = dir * distanceFromPlayer;
    }

    public void Init(Unit unit, Transform parent)
    {
        _unit = unit;

        _rig.bodyType = RigidbodyType2D.Kinematic;
        _rig.simulated = false;

        SetParent(parent);
    }

    public void SetParent(Transform parent)
    {
        _mainTf.parent = parent;
        _mainTf.localPosition = Vector2.zero;
    }

    public void Unparent()
    {
        _mainTf.parent = null;
    }

    public virtual bool Shoot(Transform target)
    {
        Debug.Log("Shooted Transform");
        return false;
    }

    /*public virtual bool Shoot(Vector2 targetPos)
    {
        Debug.Log("Shooted Pos");
        return false;
    }*/

    public virtual bool AIShoot(Unit targetUnit)
    {
        Debug.Log("AI Shooted");
        return false;
    }

    public virtual void DropWeapon()
    {
        Unparent();
        StopAllCoroutines();

        _unit = null;

        _rig.bodyType = RigidbodyType2D.Dynamic;
        _rig.simulated = true;
        _rig.angularVelocity = 480f;
    }

    public void DropWeapon(Vector2 dropVector, float power, float distanceFromUnit)
    {
        DropWeapon();

        Vector2 dropDir = dropVector.normalized;

        Vector2 spawnPos = (Vector2)_mainTf.position + (dropDir * distanceFromUnit);

        _mainTf.position = spawnPos;

        _rig.velocity = dropDir * power;
    }

    public virtual void ThrowWeapon(Vector2 targetPos)
    {
        Unparent();
        StopAllCoroutines();

        _weaponState = WeaponState.Flying;
        _mainTf.gameObject.layer = 9;

        LayerMask ground = 3;
        LayerMask platform = 10;
        LayerMask player = 8;
        LayerMask enemy = 7;

        Vector2 theTargetPos = targetPos;

        bool value = false;

        Collider2D[] enemyHitsOnTargetPos = Physics2D.OverlapCircleAll(theTargetPos, 1f, LayerMask.GetMask("Enemy"));
        foreach(Collider2D col in enemyHitsOnTargetPos)
        {
            if(col.gameObject.layer == enemy && col.gameObject != _unit.gameObject)
            {
                theTargetPos = col.transform.position;
                value = true;
                break;
            }
        }
        Debug.Log($"Has target changed: {value} | Target Pos: {theTargetPos}");

        float distanceFromPlayer = 1.25f;
        //Vector2 dir = (theTargetPos - (Vector2)_mainTf.position).normalized;

        Vector2 dir = (theTargetPos - (Vector2)_unit.transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(_unit.transform.position, dir, distanceFromPlayer, ground + platform);
        if (hit.transform != null)
            distanceFromPlayer = 0f;

        //Vector2 spawnPos = (Vector2)_mainTf.position + (dir * distanceFromPlayer);
        Vector2 spawnPos = (Vector2)_unit.transform.position + (dir * distanceFromPlayer);
        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        _mainTf.position = spawnPos;
        _mainTf.rotation = Quaternion.Euler(0, 0, rotZ);

        _rig.simulated = true;

        float speed = 15f;
        _rig.velocity = dir * speed;
        _rig.angularVelocity = 1440f;

        _unit = null;
    }

    private void _weaponWrapper_OnCollisionTouch(object sender, Collision2D collision)
    {
        if (_weaponState != WeaponState.Flying)
            return;

        _weaponState = WeaponState.Laying;
        _mainTf.gameObject.layer = 12;

        _rig.bodyType = RigidbodyType2D.Dynamic;
        _rig.velocity *= .2f;
        _rig.angularVelocity *= .2f;

        Vector2 dirToWeapon = (transform.position - collision.transform.position).normalized;
        Vector2 dirToCollision = -dirToWeapon;
        float sign = Mathf.Sign(dirToWeapon.x);

        Rigidbody2D rig = collision.transform.GetComponent<Rigidbody2D>();
        if(rig != null)
        {
            rig.velocity = dirToCollision * 3f;
        }

        Unit unit = collision.transform.GetComponent<Unit>();
        if (unit != null && !unit.IsPlayer)
        {
            unit.Enemy.Stun(2f);

            unit.MyWeaponController.DropWeapon(new Vector2(sign, 0f), 3f, .5f);

            SoundManager.Instance.Play("HitInFlight");
        }
        else
        {
            SoundManager.Instance.Play("HitWall");
        }
    }

    public virtual bool IsEmpty()
    {
        return false;
    }

    public virtual bool IsFlying()
    {
        return _weaponState == WeaponState.Flying;
    }

    public virtual bool IsAttacking()
    {
        return false;
    }

    public virtual WeaponType GetWeaponType()
    {
        Debug.LogError("Not Implemented");
        return WeaponType.Mellee;
    }

    public virtual int GetCurrentAmmo()
    {
        Debug.LogError("Not Implemented");
        return 0;
    }

    public virtual WeaponParams GetWeaponParams()
    {
        Debug.LogError("Not Implemented");
        return null;
    }

    public virtual void ResetAmmo() { }

    public SpriteRenderer GetSpriteRenderer()
    {
        return _spriteRenderer;
    }

    

    public void SetAttackMask(LayerMask attackMask)
    {
        _attackMask = attackMask;
    }

    protected Vector2 FindMousePosInWorld()
    {
        Camera mainCamera = Camera.main;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 mousePosInWorld = mainCamera.ScreenToWorldPoint(mousePos);

        return mousePosInWorld;
    }
}
