using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTrigger : MonoBehaviour
{
    public event EventHandler<Collider2D> OnPlayerJumpedOnHead;
    public event EventHandler<Collider2D> OnPlayerJumpedOffHead;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            OnPlayerJumpedOnHead?.Invoke(this, collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            OnPlayerJumpedOffHead?.Invoke(this, collision);
        }
    }
}
