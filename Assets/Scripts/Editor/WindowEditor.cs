using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Window))]
public class WindowEditor : Editor
{
    Vector2 _size;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Window window = target as Window;

        SpriteRenderer renderer = window.GetComponent<SpriteRenderer>();

        BoxCollider2D collider = window.GetComponent<BoxCollider2D>();

        _size = collider.size;

        _size = EditorGUILayout.Vector2Field("Size:", _size);

        collider.size = _size;
        renderer.size = _size;
    }
}
