using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingSound : PlayerComponentGetter
{
    [SerializeField] private float _stepEvery = .2f;
    private float _timer;
    private void Update()
    {
        if (Mathf.Abs(_rig.velocity.x) >= .4f && _player.IsGrounded)
        {
            _timer += Time.deltaTime;
            if (_timer >= _stepEvery)
            {
                SoundManager.Instance.Play("Step");
                _timer -= _stepEvery;
            }
        }
    }
}
