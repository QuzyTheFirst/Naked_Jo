using UnityEngine;
using UnityEditor;

public class RounderWindow : EditorWindow
{
    [MenuItem("Window/MyWindows/Rounder")]
    public static void ShowWindow()
    {
        GetWindow<RounderWindow>("Rounder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Round position to .5!");

        if (GUILayout.Button("ROUND!"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Vector2 currentPos = obj.transform.position;

                Vector2 newPos = new Vector2(Mathf.Round(currentPos.x * 2) * .5f, Mathf.Round(currentPos.y * 2) * .5f);
                
                obj.transform.position = newPos;
            }
        }
    }
}
