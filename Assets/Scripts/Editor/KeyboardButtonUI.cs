using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(KeyboardButtonUI))]
public class KeyboardButtonUIEditor : Editor
{
    private Vector2 _size;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        KeyboardButtonUI keyboardButton = target as KeyboardButtonUI;

        RectTransform frame = keyboardButton.GetComponent<RectTransform>();
        RectTransform background = keyboardButton.BackgroundImageRTF;
        RectTransform buttonText = keyboardButton.ButtonTextRTF;

        ChangeSize(frame, background, buttonText);
    }

    private void ChangeSize(RectTransform frame, RectTransform background, RectTransform buttonText)
    {
        _size = frame.rect.size;

        _size = EditorGUILayout.Vector2Field("Size:", _size);

        frame.sizeDelta = _size;
        background.sizeDelta = _size;
        buttonText.sizeDelta = _size - Vector2.one * 20;
    }
}
