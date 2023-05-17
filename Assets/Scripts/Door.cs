using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum OpenDoorDirection
    {
        Right = 1,
        Left = -1
    }

    [SerializeField] private Sprite _openedSprite;

    protected BoxCollider2D _boxCollider;
    protected SpriteRenderer _spriteRenderer;

    protected void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void OpenDoor(OpenDoorDirection openDoorDirection)
    {
        if(openDoorDirection == OpenDoorDirection.Right)
            _spriteRenderer.flipX = true;

        SoundManager.Instance.Play("DoorOpen");

        _spriteRenderer.sprite = _openedSprite;
        _boxCollider.enabled = false;
    }
}
