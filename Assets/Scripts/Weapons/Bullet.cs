using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float hitPower;

    private Vector2 _shootPos;

    private Rigidbody2D _rig;

    public Vector2 ShootPos { get { return _shootPos; } set { _shootPos = value; } }
    public Rigidbody2D Rig { get { return _rig; } }

    private void Awake()
    {
        _rig = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamagable iDamagable = collision.transform.GetComponent<IDamagable>();
        if (iDamagable != null)
        {
            iDamagable.Damage(0f);
        }

        Rigidbody2D rig = collision.transform.GetComponent<Rigidbody2D>();
        if(rig != null)
        {
            Vector2 dir = ((Vector2)collision.transform.position - _shootPos).normalized;
            rig.velocity = dir * hitPower;
        }

        Destroy(gameObject);
    }

}
