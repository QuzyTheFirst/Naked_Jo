using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponWrapper : MonoBehaviour
{
    public event EventHandler<Collision2D> OnCollisionTouch;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionTouch?.Invoke(this, collision);
    }
}
