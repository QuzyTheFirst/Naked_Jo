using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private float hitPower;

    private Vector2 _shootPos;

    private Rigidbody2D _rig;

    private bool _hittedTarget = false;

    [Header("Deflection")]
    [SerializeField] private float _deflectionDuration = .5f;
    private float _deflectionTimer;
    private bool _isDeflected;

    public Vector2 ShootPos { get { return _shootPos; } set { _shootPos = value; } }
    public Rigidbody2D Rig { get { return _rig; } }
    public bool IsDeflected { 
        get { 
            return _isDeflected; 
        } 
        set { 
            _deflectionTimer = _deflectionDuration;
            _shootPos = transform.position;
            _isDeflected = value; 
        } 
    }

    private void Awake()
    {
        _rig = GetComponent<Rigidbody2D>();
        _shootPos = transform.position;
    }
    private void Update()
    {
        if(_deflectionTimer > 0f && _isDeflected == true)
        {
            _deflectionTimer -= Time.deltaTime;
        }
        else if(_deflectionTimer <= 0f && _isDeflected == true)
        {
            _isDeflected = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;

        if (_hittedTarget)
            return;
        _hittedTarget = true;

        IDamagable iDamagable = collision.transform.GetComponent<IDamagable>();
        if (iDamagable != null)
        {
            iDamagable.Damage(_shootPos, 1);
            SoundManager.Instance.Play("Hit");
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
