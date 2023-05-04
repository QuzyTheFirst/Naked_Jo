using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelleeWeapon : Weapon
{
    [SerializeField] private MelleeWeaponParams _weaponParams;

    private float _lastAttackTime;
    private float _attackTimer = 0;

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
        MainTf.localPosition = dir * distanceFromPlayer;
    }

    public override bool Shoot(Transform target)
    {
        if (Time.time < _lastAttackTime + _weaponParams.PlayerAttackRate)
            return false;

        //Debug.Log("Attack Transform");

        _isAttacking = true;

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

        while (_attackTimer < _weaponParams.AttackTime)
        {
            dir = (target.position - UnitController.transform.position).normalized;

            CanDeflectBulletsAbility(target.transform.position);

            _attackTimer += Time.fixedDeltaTime;
            Collider2D[] hitObjs = Physics2D.OverlapCircleAll((Vector2)UnitController.transform.position + dir * _weaponParams.AttackDistance, _weaponParams.AttackRange, AttackMask);

            foreach (Collider2D obj in hitObjs)
            {
                if (obj.transform == UnitController.transform)
                    continue;

                RaycastHit2D hit = Physics2D.Raycast(UnitController.transform.position, dir);
                if (hit.transform != obj.transform)
                    continue;

                IDamagable iDamagable = obj.GetComponent<IDamagable>();

                if (iDamagable != null)
                {
                    _hitted = true;
                    iDamagable.Damage(60f);
                }
            }

            if (_hitted)
            {
                SoundManager.Instance.Play("Hit");
                _hitted = false;
            }

            yield return new WaitForFixedUpdate();
        }
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
            Collider2D[] hitObjs = Physics2D.OverlapCircleAll((Vector2)UnitController.transform.position + dir * _weaponParams.AttackDistance, _weaponParams.AttackRange, AttackMask);

            foreach (Collider2D obj in hitObjs)
            {
                if (obj.transform == UnitController.transform)
                    continue;

                RaycastHit2D hit = Physics2D.Raycast(UnitController.transform.position, dir);
                if (hit.transform != obj.transform)
                    continue;

                IDamagable iDamagable = obj.GetComponent<IDamagable>();

                if (iDamagable != null)
                {
                    _hitted = true;
                    iDamagable.Damage(60f);
                }
            }

            if (_hitted)
            {
                SoundManager.Instance.Play("Hit");
                _hitted = false;
            }

            yield return new WaitForFixedUpdate();
        }
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
        foreach(Collider2D bullet in bullets)
        {
            Rigidbody2D rig = bullet.GetComponent<Rigidbody2D>();
            float speed = rig.velocity.magnitude;
            Vector2 newVector = target - (Vector2)UnitController.transform.position;
            bullet.GetComponent<Rigidbody2D>().velocity = newVector.normalized * speed;
        }
    }

    public override WeaponType GetWeaponType()
    {
        return WeaponType.Mellee;
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
