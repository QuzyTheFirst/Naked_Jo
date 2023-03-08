using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DeathZone))]
public class DeathZoneEditor : Editor
{
    private Vector2 _size;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DeathZone deathZone = target as DeathZone;

        BoxCollider2D collider = deathZone.GetComponent<BoxCollider2D>();

        ChangeSize(collider);
    }

    private void ChangeSize(BoxCollider2D collider)
    {
        _size = collider.size;

        _size = EditorGUILayout.Vector2Field("Size:", _size);

        collider.size = _size;
    }
}
