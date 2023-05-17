using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDoor : Door
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != 8)
            return;

        Vector2 dir = transform.position - collision.transform.position;
        Debug.Log(dir);
        float sign = Mathf.Sign(dir.x);

        OpenDoor((OpenDoorDirection)sign);
    }
}
