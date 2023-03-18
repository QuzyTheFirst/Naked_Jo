using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamagable iDamagable = collision.GetComponent<IDamagable>();
        if(iDamagable != null)
        {
            iDamagable.Damage(0f);
        }
    }
}
