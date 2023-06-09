using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Platform))]
public class PlatformEditor : Editor
{
    Vector2 _size;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Platform platform = target as Platform;

        SpriteRenderer renderer = platform.GetComponent<SpriteRenderer>();

        BoxCollider2D collider = platform.GetComponent<BoxCollider2D>();

        _size = collider.size;

        _size = EditorGUILayout.Vector2Field("Size:", _size);

        collider.size = _size;
        renderer.size = _size;
        platform.PlatformSpriteMask.localScale = _size;
    }
}
