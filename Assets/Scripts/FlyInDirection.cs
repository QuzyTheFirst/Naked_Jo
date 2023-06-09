using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyInDirection : MonoBehaviour
{
    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _power;
    [SerializeField] private Rigidbody2D _rig;

    private void Awake()
    {
        _rig.velocity = _direction.normalized * _power;
    }
}
