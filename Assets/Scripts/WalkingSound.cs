using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingSound : MonoBehaviour
{
    [SerializeField] private float _stepEverySeconds = .2f;
    private float _timer;

    private Vector2 _lastStepPos;
    private PlayerController _player;

    private void Awake()
    {
        _player = transform.GetComponentInChildren<PlayerController>();
    }

    private void Update()
    {
        if (_player.IsWalking && _player.IsGrounded)
        {
            _timer += Time.deltaTime;
            if (_timer >= _stepEverySeconds)
            {
                PlayStepSound();
            }
        }
    }

    private void PlayStepSound()
    {
        SoundManager.Instance.Play("Step");
        _lastStepPos = transform.position;
        _timer -= _stepEverySeconds;
    }
}
