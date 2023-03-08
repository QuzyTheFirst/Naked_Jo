using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Sprite _openedSprite;

    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != 8)
            return;

        Vector2 dir = transform.position - collision.transform.position;
        float sign = Mathf.Sign(dir.x);

        OpenDoor(sign);
    }

    private void OpenDoor(float value)
    {
        if(value > 0)
            _spriteRenderer.flipX = true;
        
        _spriteRenderer.sprite = _openedSprite;
        _boxCollider.enabled = false;
    }
}
