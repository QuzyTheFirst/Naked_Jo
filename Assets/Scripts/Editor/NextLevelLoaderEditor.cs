using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NextLevelLoader))]
public class NextLevelLoaderEditor : Editor
{
    private Vector2 _size;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NextLevelLoader loader = target as NextLevelLoader;

        BoxCollider2D collider = loader.GetComponent<BoxCollider2D>();
        SpriteRenderer spriteRenderer = loader.SpriteRenderer;

        ChangeSize(collider, spriteRenderer);
    }

    private void ChangeSize(BoxCollider2D collider, SpriteRenderer spriteRenderer)
    {
        _size = collider.size;

        _size = EditorGUILayout.Vector2Field("Size:", _size);

        collider.size = _size;
        spriteRenderer.size = _size;
    }
}
