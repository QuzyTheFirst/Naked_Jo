using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelleeWeapon : Weapon
{
    [SerializeField] private MelleeWeaponParams _weaponParams;

    private float _lastAttackTime;
    private float _attackTimer = 0;

    private Animator _anim;

    private Vector2 dir;

    private new void Awake()
    {
        base.Awake();
        _anim = GetComponent<Animator>();
    }

    public override void Shoot(Transform target)
    {
        if (Time.time < _lastAttackTime + _weaponParams.AttackRate)
            return;

        _anim.SetTrigger("Prepare");

        StartCoroutine(Attack(target, _weaponParams.PrepareTime));

        _lastAttackTime = Time.time;
    }

    private IEnumerator Attack(Vector2 targetPos, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        _anim.SetTrigger("Attack");

        dir = (targetPos - (Vector2)UnitController.transform.position).normalized;
        
        while (_attackTimer < _weaponParams.AttackTime)
        {
            _attackTimer += Time.fixedDeltaTime;
            Collider2D[] hitObjs = Physics2D.OverlapCircleAll((Vector2)UnitController.transform.position + dir * _weaponParams.AttackDistance, _weaponParams.AttackRange, AttackMask);

            foreach (Collider2D obj in hitObjs)
            {
                if (obj.transform == UnitController.transform)
                    continue;

                IDamagable iDamagable = obj.GetComponent<IDamagable>();

                if (iDamagable != null)
                {
                    iDamagable.Damage(60f);
                }
            }
            yield return new WaitForFixedUpdate();
        }

        _attackTimer = 0;
    }


    private IEnumerator Attack(Transform target, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        if (target == null)
            yield break;

        _anim.SetTrigger("Attack");

        dir = (target.position - UnitController.transform.position).normalized;

        while (_attackTimer < _weaponParams.AttackTime)
        {
            _attackTimer += Time.fixedDeltaTime;
            Collider2D[] hitObjs = Physics2D.OverlapCircleAll((Vector2)UnitController.transform.position + dir * _weaponParams.AttackDistance, _weaponParams.AttackRange, AttackMask);

            foreach (Collider2D obj in hitObjs)
            {
                if (obj.transform == UnitController.transform)
                    continue;

                IDamagable iDamagable = obj.GetComponent<IDamagable>();

                if (iDamagable != null)
                {
                    iDamagable.Damage(60f);
                }
            }
            yield return new WaitForFixedUpdate();
        }

        _attackTimer = 0;
    }

    public override void DropWeapon()
    {
        base.DropWeapon();

        _anim.SetTrigger("Idle");
    }

    public override float GetAttackDistance()
    {
        return _weaponParams.DistanceBeforeAttack;
    }

    private void OnDrawGizmos()
    {
        if (UnitController != null)
            Gizmos.DrawWireSphere((Vector2)UnitController.transform.position + dir * _weaponParams.AttackDistance, _weaponParams.AttackRange);
        else
            Gizmos.DrawWireSphere((Vector2)transform.position * _weaponParams.AttackDistance, _weaponParams.AttackRange);
    }
}
