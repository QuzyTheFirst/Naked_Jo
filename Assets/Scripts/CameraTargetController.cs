using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetController : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Range(0f, 1f)]
    [SerializeField] private float targetPosLerp =  .4f;

    [SerializeField] private float maxDistance = 4;

    public void UpdateTargetPos(Vector2 playerPos)
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector2 dir = (cursorPos - playerPos).normalized;

        float distance = Vector2.Distance(cursorPos, playerPos);
        distance = Mathf.Min(distance, maxDistance);
        //Debug.Log("Distance: " + distance);

        target.position = playerPos +  dir * distance * targetPosLerp;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, maxDistance * targetPosLerp);
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
