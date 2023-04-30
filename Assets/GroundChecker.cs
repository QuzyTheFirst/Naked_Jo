using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : ComponentsGetter
{
    [Header("Ground Check")]
    [SerializeField] private float _checkDistance = .1f;
    [SerializeField] private LayerMask _groundMask;

    private bool _isGrounded;

    public bool IsGrounded { get { return _isGrounded; } }

    private void Awake()
    {
        base.GetAllComponents(false);
    }

    private void FixedUpdate()
    {
        _isGrounded = GroundCheck();
    }

    private bool GroundCheck()
    {
        return Physics2D.CircleCast(transform.position, _col.radius, Vector2.down, _checkDistance, _groundMask);
    }
}
