using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorWithKey : MonoBehaviour
{
    [SerializeField] private Key.KeyType _keyType;

    private BoxCollider2D _boxCollider;
    private Animator _anim;
    private SpriteRenderer _spriteRenderer;

    public Key.KeyType KeyType { get { return _keyType; } }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OpenDoor(float value)
    {
        _anim.SetTrigger("Open");

        if (value > 0)
            _spriteRenderer.flipX = true;

        SoundManager.Instance.Play("DoorOpen");

        _boxCollider.enabled = false;
    }
}
