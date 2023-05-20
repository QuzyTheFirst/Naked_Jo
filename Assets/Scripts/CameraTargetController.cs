using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetController : MonoBehaviour
{
    private const float TARGET_POS_STANDART_LERP = .4f;
    private const float TARGET_POS_MAX_LERP = 1f;

    [SerializeField] private Transform target;

    [Range(0f, 1f)]
    [SerializeField] private float _targetPosLerp =  .4f;

    [SerializeField] private float _maxDistance = 4;

    public void UpdateTargetPos(Vector2 playerPos)
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector2 dir = (cursorPos - playerPos).normalized;

        float distance = Vector2.Distance(cursorPos, playerPos);
        distance = Mathf.Min(distance, _maxDistance);
        //Debug.Log("Distance: " + distance);

        target.position = playerPos +  dir * distance * _targetPosLerp;
    }

    public void ToggleTargetPosLerp(bool value)
    {
        if (value)
            _targetPosLerp = TARGET_POS_MAX_LERP;
        else
            _targetPosLerp = TARGET_POS_STANDART_LERP;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _maxDistance * _targetPosLerp);
        Gizmos.DrawWireSphere(transform.position, _maxDistance);
    }
}
