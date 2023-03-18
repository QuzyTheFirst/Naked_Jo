using UnityEngine;
using UnityEditor;

public class SortingWindow : EditorWindow
{
    [MenuItem("Window/MyWindows/Sorting")]
    public static void ShowWindow()
    {
        GetWindow<SortingWindow>("Sorting");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sort your level!");

        if (GUILayout.Button("Sort!"))
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            GameObject mainFolder = CreateOrFindFolder("Sorted Folder", allObjects);


        }
    }

    private GameObject CreateOrFindFolder(string folderName, GameObject[] objects)
    {
        GameObject folder = null;

        foreach (GameObject go in objects)
        {
            if (go.name.Contains(folderName))
            {
                folder = go;
                break;
            }
        }

        if (folder == null)
            folder = new GameObject(folderName);

        return folder;
    }
}
