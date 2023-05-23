using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentsGetter : MonoBehaviour
{
    private CircleCollider2D _col;
    private Rigidbody2D _rig;

    private Unit _unit;

    private PlayerController _playerController;
    private AIBase _enemyController;

    private WeaponController _weaponController;

    private GroundChecker _groundChecker;

    private HeadTrigger _headTrigger;

    private Flip _flip;

    private SpriteRenderer _spriteRenderer;

    private KeyHolder _keyHolder;

    private CostumeChanger _costumeChanger;

    public CircleCollider2D MyCircleCollider { get { return _col; } }
    public Rigidbody2D MyRigidbody { get { return _rig; } }
    public Unit MyUnit { get { return _unit; } }
    public PlayerController MyPlayerController { get { return _playerController; } }
    public AIBase MyEnemyController { get { return  _enemyController; } } 
    public WeaponController MyWeaponController { get { return _weaponController; } }
    public GroundChecker MyGroundChecker { get { return _groundChecker; } }
    public HeadTrigger MyHeadTrigger { get { return _headTrigger; } }
    public Flip MyFlip { get { return _flip; } }
    public SpriteRenderer MySpriteRenderer { get { return _spriteRenderer; } }
    public KeyHolder MyKeyHolder { get { return _keyHolder; } }
    public CostumeChanger MyCostumeChanger { get { return _costumeChanger; } }
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

        _unit = startPoint.GetComponent<Unit>();

        _playerController = startPoint.GetComponentInChildren<PlayerController>();
        _enemyController = startPoint.GetComponentInChildren<AIBase>();

        _weaponController = startPoint.GetComponentInChildren<WeaponController>();

        _groundChecker = startPoint.GetComponentInChildren<GroundChecker>();

        _headTrigger = startPoint.GetComponentInChildren<HeadTrigger>();

        _flip = startPoint.GetComponent<Flip>();

        _spriteRenderer = startPoint.Find("Graphics").GetComponent<SpriteRenderer>();

        _keyHolder = startPoint.GetComponent<KeyHolder>();

        _costumeChanger = _spriteRenderer.GetComponent<CostumeChanger>();
    }
}
