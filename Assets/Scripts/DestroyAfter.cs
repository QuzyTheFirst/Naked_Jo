using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _delay;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        Color targetColor = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0f);

        LeanTween.color(gameObject, targetColor, _lifeTime).setDelay(_delay).setOnComplete(() => {
            Destroy(gameObject);
        });
    }
}
