using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flip : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = transform.Find("Graphics").GetComponent<SpriteRenderer>();
    }

    public void TryToFlip(float x)
    {
        if (_spriteRenderer == null)
        {
            Debug.LogWarning($"There is no sprite renderer on {transform.name}");
            return;
        }

        if(x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if(x < 0)
        {
            _spriteRenderer.flipX = true;
        }
    }
}
