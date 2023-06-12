using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleKatanaManBulletDeflectState : LittleKatanaManBaseState
{
    public LittleKatanaManBulletDeflectState(LittleKatanaMan context, LittleKatanaManStateFactory factory) : base(context, factory) { }

    private Vector2 _lastBulletPos;

    public override void OnEnter(LittleKatanaMan context)
    {
        context.Movement = AIBase.MovementState.Stop;

        context.MyWeaponController.GetAnimator().SetTrigger("Block");

        if (context.BulletsToDeflect.Length > 0)
        {
            DeflectBullets(context, context.BulletsToDeflect);
        }
    }

    public override void OnUpdate(LittleKatanaMan context)
    {
        context.BulletDeflectionTimer -= Time.fixedDeltaTime;

        if (context.BulletsToDeflect.Length > 0)
        {
            DeflectBullets(context, context.BulletsToDeflect);
        }

        UpdateWeaponTargetPos(context);

        CheckSwitchStates(context);
    }

    public override void CheckSwitchStates(LittleKatanaMan context)
    {
        if (context.StunTime > 0f)
        {
            SwitchState(Factory.Stun());
            return;
        }

        if (context.BulletDeflectionTimer > 0f)
            return;

        if (context.TargetUnit == null)
        {
            SwitchState(Factory.Patrol());
            return;
        }

        float distance = Vector2.Distance(context.transform.position, context.TargetUnitTf.position);
        if (distance > context.AttackRadius || !context.CanISeeMyTarget)
        {
            SwitchState(Factory.Chase());
            return;
        }
    }

    public override void OnExit(LittleKatanaMan context)
    {
        context.MyWeaponController.GetAnimator().SetTrigger("Idle");
    }

    public override void InitializeSubState(LittleKatanaMan context)
    {

    }

    private void UpdateWeaponTargetPos(LittleKatanaMan context)
    {
        context.MyWeaponController.TargetPos = _lastBulletPos;
    }

    private void DeflectBullets(LittleKatanaMan context, Collider2D[] bullets)
    {
        foreach (Collider2D col in bullets)
        {
            Bullet bullet = col.GetComponent<Bullet>();

            if (bullet.IsDeflected)
                continue;

            bullet.IsDeflected = true;

            Rigidbody2D rig = bullet.Rig;

            float randNum = Random.Range(context.BulletSpreadAmount, -context.BulletSpreadAmount);
            float speed = 20;

            Vector2 perpendicular = Vector2.Perpendicular(rig.velocity);
            Vector2 newDir = (-rig.velocity + perpendicular * randNum).normalized;

            float rotZ = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg;

            bullet.transform.rotation = Quaternion.Euler(0, 0, rotZ);

            rig.velocity = newDir * speed;

            _lastBulletPos = bullet.transform.position;
        }

        context.BulletsToDeflect = new Collider2D[0];
    }
}
