using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KeyboardButton))]
public class KeyboardButtonEditor : Editor
{
    private Vector2 _size;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        KeyboardButton keyboardButton = target as KeyboardButton;

        SpriteRenderer frame = keyboardButton.GetComponent<SpriteRenderer>();
        SpriteRenderer background = keyboardButton.BackgroundSprite;

        ChangeSize(frame, background);
    }

    private void ChangeSize(SpriteRenderer frame, SpriteRenderer background)
    {
        _size = frame.size;

        _size = EditorGUILayout.Vector2Field("Size:", _size);

        frame.size = _size;
        background.size = _size;
    }
}
