using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentsGetter : MonoBehaviour
{
    protected CircleCollider2D _col;
    protected Rigidbody2D _rig;

    protected PlayerController _playerController;
    protected SimpleEnemy _enemyController;

    protected WeaponController _weaponController;

    protected GroundChecker _groundChecker;

    protected HeadTrigger _headTrigger;

    protected Flip _flip;

    protected SpriteRenderer _spriteRenderer;

    protected KeyHolder _keyHolder;

    protected void GetAllComponents(bool isParent)
    {
        Transform startPoint;

        if (isParent)
        {
            startPoint = transform;
        }
        else
        {
            startPoint = transform.parent;
        }

        _col = startPoint.GetComponent<CircleCollider2D>();
        _rig = startPoint.GetComponent<Rigidbody2D>();

        _playerController = startPoint.GetComponentInChildren<PlayerController>();
        _enemyController = startPoint.GetComponentInChildren<SimpleEnemy>();

        _weaponController = startPoint.GetComponentInChildren<WeaponController>();

        _groundChecker = startPoint.GetComponentInChildren<GroundChecker>();

        _headTrigger = startPoint.GetComponentInChildren<HeadTrigger>();

        _flip = startPoint.GetComponent<Flip>();

        _spriteRenderer = startPoint.Find("Graphics").GetComponent<SpriteRenderer>();

        _keyHolder = startPoint.GetComponent<KeyHolder>();
    }
}
