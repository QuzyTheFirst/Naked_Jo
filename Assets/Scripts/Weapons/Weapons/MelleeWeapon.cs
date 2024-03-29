using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelleeWeapon : Weapon
{
    [SerializeField] private MelleeWeaponParams _weaponParams;

    private float _lastAttackTime;
    private float _attackTimer = 0f;
    private float _endAttackTimer = 0f;

    private bool _isAttacking = false;
    private float _attackTime;

    private Animator _anim;

    private Vector2 dir;

    private bool _hitted = false;

    private new void Awake()
    {
        base.Awake();
        _anim = GetComponent<Animator>();
    }

    public override void OnUpdate(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)UnitController.transform.position).normalized;

        //Weapon Rotation
        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        MainTf.rotation = Quaternion.Euler(0, 0, rotZ);

        //Weapon Position
        float distanceFromPlayer = _weaponParams.WeaponDistanceFromUnit;
        MainTf.localPosition = (dir * distanceFromPlayer);

        if (_handle != null)
            MainTf.localPosition -= new Vector3(0f, _handle.localPosition.y);
    }

    public override bool Shoot(Transform target)
    {
        if (Time.time < _lastAttackTime + _weaponParams.PlayerAttackRate)
            return false;

        //Debug.Log("Attack Transform");

        _isAttacking = true;

        if(_weaponParams.PrepareTime > 0)
            _anim.SetTrigger("Prepare");

        StartCoroutine(Attack(target, _weaponParams.PrepareTime));

        _lastAttackTime = Time.time;

        return true;
    }

    private IEnumerator Attack(Transform target, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        if (target == null)
            yield break;

        _anim.SetTrigger("Attack");

        SoundManager.Instance.Play(_weaponParams.AttackSoundName);

        _attackTimer = 0f;
        _endAttackTimer = 0f;

        List<Collider2D> attackedColliders = new List<Collider2D>();

        while (_attackTimer < _weaponParams.AttackTime)
        {
            dir = (target.position - UnitController.transform.position).normalized;

            CanDeflectBulletsAbility(target.transform.position);

            _attackTimer += Time.fixedDeltaTime;

            Collider2D[] hitColls = Physics2D.OverlapCircleAll((Vector2)UnitController.transform.position + dir * _weaponParams.AttackDistance, _weaponParams.AttackRange, AttackMask);
            //Debug.Log($"Damaged {hitObjs.Length} units");
            foreach (Collider2D coll in hitColls)
            {
                if (coll.transform == UnitController.transform || attackedColliders.Contains(coll))
                    continue;

                RaycastHit2D hit = Physics2D.Raycast(UnitController.transform.position, dir, _weaponParams.AttackDistance + _weaponParams.AttackRange, LayerMask.GetMask("Ground") + LayerMask.GetMask("Platform") + AttackMask);
                if (hit.transform != coll.transform)
                    continue;

                IDamagable iDamagable = coll.GetComponent<IDamagable>();
                if (iDamagable != null)
                {
                    _hitted = true;
                    iDamagable.Damage(UnitController.transform.position, 1);
                    attackedColliders.Add(coll);
                }

                Rigidbody2D rig = coll.GetComponent<Rigidbody2D>();
                if(rig != null)
                {
                    rig.velocity = dir * 6;
                }
            }

            if (_hitted)
            {
                SoundManager.Instance.Play("Hit");
                _hitted = false;
            }

            yield return new WaitForFixedUpdate();
        }

        _anim.SetTrigger("EndAttack");

        while (_endAttackTimer < _weaponParams.EndAttackTime)
        {
            yield return new WaitForFixedUpdate();
            _endAttackTimer += Time.fixedDeltaTime;
        }

        _anim.SetTrigger("Idle");

        _isAttacking = false;
    }

    public override bool AIShoot(Unit targetUnit)
    {
        if (Time.time < _lastAttackTime + _weaponParams.PlayerAttackRate)
            return false;

        //Debug.Log("AI Attack");

        _isAttacking = true;

        _anim.SetTrigger("Prepare");

        StartCoroutine(AIAttack(targetUnit, _weaponParams.PrepareTime));

        _lastAttackTime = Time.time;

        return true;
    }

    private IEnumerator AIAttack(Unit targetUnit, float timeToWait)
    {
        Vector2 startedPos = targetUnit.transform.position;
        bool hasStartedRolling = targetUnit.Player.IsRolling;

        yield return new WaitForSeconds(timeToWait);

        _anim.SetTrigger("Attack");

        SoundManager.Instance.Play(_weaponParams.AttackSoundName);

        _attackTimer = 0f;
        _endAttackTimer = 0f;

        List<Collider2D> attackedColliders = new List<Collider2D>();

        while (_attackTimer < _weaponParams.AttackTime)
        {
            if (targetUnit == null)
                break;

            if (targetUnit.Player.IsRolling)
                hasStartedRolling = true;

            //Debug.Log("Has started Rolling: " + hasStartedRolling);
            //Debug.Log($"Started Pos: {startedPos}, TargetUnit: {targetUnit.transform.position}");

            if(!hasStartedRolling)
                dir = (targetUnit.transform.position - UnitController.transform.position).normalized;
            else
                dir = (startedPos - (Vector2)UnitController.transform.position).normalized;

            CanDeflectBulletsAbility(targetUnit.transform.position);

            _attackTimer += Time.fixedDeltaTime;
            Collider2D[] hitColls = Physics2D.OverlapCircleAll((Vector2)UnitController.transform.position + dir * _weaponParams.AttackDistance, _weaponParams.AttackRange, AttackMask);

            foreach (Collider2D coll in hitColls)
            {
                if (coll.transform == UnitController.transform || attackedColliders.Contains(coll))
                    continue;

                RaycastHit2D hit = Physics2D.Raycast(UnitController.transform.position, dir, _weaponParams.AttackDistance + _weaponParams.AttackRange, LayerMask.GetMask("Ground") + LayerMask.GetMask("Platform") + AttackMask);
                if (hit.transform != coll.transform)
                    continue;

                IDamagable iDamagable = coll.GetComponent<IDamagable>();

                if (iDamagable != null)
                {
                    _hitted = true;
                    iDamagable.Damage(UnitController.transform.position, 1);
                    attackedColliders.Add(coll);
                }

                Rigidbody2D rig = coll.GetComponent<Rigidbody2D>();
                if (rig != null)
                {
                    rig.velocity = dir * 6;
                }
            }

            if (_hitted)
            {
                SoundManager.Instance.Play("Hit");
                _hitted = false;
            }

            yield return new WaitForFixedUpdate();
        }

        _anim.SetTrigger("EndAttack");

        while (_endAttackTimer < _weaponParams.EndAttackTime)
        {
            yield return new WaitForFixedUpdate();
            _endAttackTimer += Time.fixedDeltaTime;
        }

        _anim.SetTrigger("Idle");

        _isAttacking = false;
    }

    private void CanDeflectBulletsAbility(Vector2 target)
    {
        if (_weaponParams.CanDeflectBullets)
        {
            Collider2D[] bullets = Physics2D.OverlapCircleAll((Vector2)UnitController.transform.position + dir * _weaponParams.AttackDistance, _weaponParams.AttackRange, LayerMask.GetMask("Bullet"));
            DeflectBullets(bullets, target);
        }
    }

    private void DeflectBullets(Collider2D[] bullets, Vector2 target)
    {
        bool deflected = false;
        foreach(Collider2D bullet in bullets)
        {
            Rigidbody2D rig = bullet.GetComponent<Rigidbody2D>();
            float speed = rig.velocity.magnitude;
            Vector2 newVector = target - (Vector2)UnitController.transform.position;
            bullet.GetComponent<Rigidbody2D>().velocity = newVector.normalized * speed;
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, newVector);
            bullet.transform.eulerAngles = new Vector3(bullet.transform.eulerAngles.x, bullet.transform.eulerAngles.y, bullet.transform.eulerAngles.z + 90f);
            deflected = true;
        }

        if (deflected)
        {
            SoundManager.Instance.Play("KatanaDeflect");
        }
    }

    public override WeaponType GetWeaponType()
    {
        return WeaponType.Mellee;
    }

    public override Animator GetAnimator()
    {
        return _anim;
    }

    public override int GetCurrentAmmo()
    {
        return -1;
    }
    public override bool IsAttacking()
    {
        return _isAttacking;
    }
    public override void DropWeapon()
    {
        base.DropWeapon();

        _anim.SetTrigger("Idle");
    }

    public override void DropWeapon(Vector2 dropVector, float power, float distanceFromUnit)
    {
        base.DropWeapon(dropVector, power, distanceFromUnit);
        Debug.Log("Set Trigger IDle");
        _anim.SetTrigger("Idle");
    }

    public override void ThrowWeapon(Vector2 targetPos)
    {
        base.ThrowWeapon(targetPos);

        _anim.SetTrigger("Idle");
    }

    public override WeaponParams GetWeaponParams()
    {
        return _weaponParams;
    }

    private void OnDrawGizmos()
    {
        if (UnitController != null)
            Gizmos.DrawWireSphere((Vector2)UnitController.transform.position + dir * _weaponParams.AttackDistance, _weaponParams.AttackRange);
        else
            Gizmos.DrawWireSphere((Vector2)transform.position * _weaponParams.AttackDistance, _weaponParams.AttackRange);
    }
}
