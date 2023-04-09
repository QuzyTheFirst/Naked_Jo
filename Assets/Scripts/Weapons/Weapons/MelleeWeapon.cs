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

    private bool _hitted = false;

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

    private IEnumerator Attack(Transform target, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        if (target == null)
            yield break;

        _anim.SetTrigger("Attack");

        SoundManager.Instance.Play(_weaponParams.AttackSoundName);

        dir = (target.position - UnitController.transform.position).normalized;

        while (_attackTimer < _weaponParams.AttackTime)
        {
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
