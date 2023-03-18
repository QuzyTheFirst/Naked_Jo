using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _possessionIcon;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void SetCursorPosition(Vector2 pos)
    {
        transform.position = pos;
    }

    public void TogglePossessionIcon(bool value)
    {
        _possessionIcon.transform.gameObject.SetActive(value);
    }

    public void ChangePossessionIconColor(Color color)
    {
        _possessionIcon.color = color;
    }
}
