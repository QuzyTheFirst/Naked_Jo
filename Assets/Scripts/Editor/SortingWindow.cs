using UnityEngine;
using UnityEditor;

[System.Serializable]
public class SortingItem
{
    public string FolderName;
    public string[] Tags;
}

public class SortingWindow : EditorWindow
{
    public SortingItem[] Items;
    SerializedObject so;

    [MenuItem("Window/MyWindows/Sorting")]
    public static void ShowWindow()
    {
        GetWindow<SortingWindow>("Sorting");
    }

    private void OnEnable()
    {
        ScriptableObject target = this;
        so = new SerializedObject(target);
    }

    private void OnGUI()
    {
        GUILayout.Label("Sort your level!");

        so.Update();
        SerializedProperty itemsProperty = so.FindProperty("Items");        

        EditorGUILayout.PropertyField(itemsProperty, true);
        so.ApplyModifiedProperties();

        ButtonSort();
    }

    private void ButtonSort()
    {
        if (GUILayout.Button("Sort!"))
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            GameObject[] selectedObject = Selection.gameObjects;

            GameObject mainFolder = CreateOrFindFolder("Sorted Folder", allObjects);

            foreach(SortingItem item in Items)
            {
                GameObject folder = CreateOrFindFolder(item.FolderName, allObjects);

                folder.transform.parent = mainFolder.transform;

                foreach(GameObject obj in selectedObject)
                {
                    foreach(string tag in item.Tags)
                    {
                        if (obj.name.ToLower().Contains(tag.ToLower())){
                            obj.transform.parent = folder.transform;
                        }
                    }
                }
            }

            SaveChanges();
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
