using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetController : MonoBehaviour
{
    private const float TARGET_POS_STANDART_LERP = .4f;
    private const float TARGET_POS_MAX_LERP = 2f;

    [SerializeField] private Transform _target;

    [Range(0f, 1f)]
    [SerializeField] private float _targetPosLerp =  .4f;

    [SerializeField] private float _maxDistance = 4;

    private bool _isCameraPositionSetted = false;
    private Vector2 _cameraStartingPos;

    public void UpdateTargetPos(Vector2 playerPos)
    {
        if (!_isCameraPositionSetted)
            _cameraStartingPos = playerPos;

        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector2 dir = (cursorPos - _cameraStartingPos).normalized;

        float distance = Vector2.Distance(cursorPos, _cameraStartingPos);
        distance = Mathf.Min(distance, _maxDistance);
        //Debug.Log("Distance: " + distance);

        _target.position = _cameraStartingPos +  dir * distance * _targetPosLerp;
    }

    public void ToggleTargetPosLerp(bool value)
    {
        if (value)
            _targetPosLerp = TARGET_POS_MAX_LERP;
        else
            _targetPosLerp = TARGET_POS_STANDART_LERP;
    }

    public void SetCameraTargetPos(Vector2 pos)
    {
        _cameraStartingPos = pos;
        _isCameraPositionSetted = true;
    }

    public void ResetCameraTargetPos()
    {
        _isCameraPositionSetted = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _maxDistance * _targetPosLerp);
        Gizmos.DrawWireSphere(transform.position, _maxDistance);
    }
}
