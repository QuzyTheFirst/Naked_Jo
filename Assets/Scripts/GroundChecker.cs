using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : ComponentsGetter
{
    [Header("Ground Check")]
    [SerializeField] private float _checkDistance = .1f;
    [SerializeField] private LayerMask _groundMask;

    private bool _isGrounded;

    private bool _oldGrounded;

    public bool IsGrounded { get { return _isGrounded; } }

    private void Awake()
    {
        base.GetAllComponents(false);
    }

    private void FixedUpdate()
    {
        _oldGrounded = _isGrounded;
        _isGrounded = GroundCheck();

        if(_oldGrounded == false && _isGrounded == true)
        {
            if(MyUnit.IsPlayer || MyEnemyController.IsPossessed)
                SoundManager.Instance?.Play("Step");
        }
    }

    private bool GroundCheck()
    {
        return Physics2D.CircleCast(transform.position, MyCircleCollider.radius, Vector2.down, _checkDistance, _groundMask);
    }
}
