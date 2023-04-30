using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponentsGetter : MonoBehaviour
{
    protected CircleCollider2D _col;
    protected Rigidbody2D _rig;
    protected PlayerController _player;
    protected WeaponController _weaponController;

    protected Flip _flip;

    protected SpriteRenderer _spriteRenderer;
    protected Animator _anim;

    protected void Awake()
    {
        _rig = GetComponent<Rigidbody2D>();
        _col = GetComponent<CircleCollider2D>();
        _player = GetComponent<PlayerController>();
        _weaponController = GetComponent<WeaponController>();

        _flip = GetComponent<Flip>();

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _anim = GetComponentInChildren<Animator>();
    }
}
