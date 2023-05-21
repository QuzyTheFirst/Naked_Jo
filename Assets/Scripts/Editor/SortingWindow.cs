using UnityEngine;
using UnityEditor;

[System.Serializable]
public class SortingItem
{
    public string FolderName;
    public string[] Tags;
}

[System.Serializable]
public class SortedItemsArray
{
    public SortingItem[] SortedItems;

    public SortedItemsArray(SortingItem[] items)
    {
        SortedItems = items;
    }
}

public class SortingWindow : EditorWindow
{
    private Vector2 _scrollPos;
    private string _fileName;
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
        EditorGUILayout.BeginVertical();
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

        GUILayout.Label("Sort your level!", EditorStyles.boldLabel);

        DrawSortingItems();

        ButtonSort();

        GUILayout.Space(30);

        GUILayout.Label("Save or load your preset.", EditorStyles.boldLabel);

        SavePreset();
        LoadPreset();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawSortingItems()
    {
        so.Update();
        SerializedProperty itemsProperty = so.FindProperty("Items");

        EditorGUILayout.PropertyField(itemsProperty, true);
        so.ApplyModifiedProperties();
    }

    private void ButtonSort()
    {
        if (GUILayout.Button("Sort!", GUILayout.Height(50)))
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            GameObject[] selectedObject = Selection.gameObjects;

            GameObject mainFolder = CreateOrFindFolder("Sorted Folder", allObjects);

            GameObject[] mainFolderChildren = new GameObject[mainFolder.transform.childCount];
            int i = 0;
            foreach(Transform tf in mainFolder.transform)
            {
                mainFolderChildren[i] = tf.gameObject;
                i++;
            }

            foreach(SortingItem item in Items)
            {
                GameObject folder = CreateOrFindFolder(item.FolderName, mainFolderChildren);

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
        }
    }

    private void SavePreset()
    {
        if (GUILayout.Button("Save Preset", GUILayout.Height(50)))
        {
            string filePath = EditorUtility.SaveFilePanel("Save preset to folder","Assets/","MyPreset.json", "json");

            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("Path is empty");
                return;
            }

            SortedItemsArray sortedItemsArray = new SortedItemsArray(Items);

            string data = JsonUtility.ToJson(sortedItemsArray, true);

            System.IO.File.WriteAllText(filePath, data);
        }
    }

    private void LoadPreset()
    {
        if (GUILayout.Button("Load Preset", GUILayout.Height(50)))
        {
            string filePath = EditorUtility.OpenFilePanel("Save preset to folder", "Assets/", "json");

            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("Path is empty");
                return;
            }

            string sortingItemsData = System.IO.File.ReadAllText(filePath);
            SortedItemsArray sortedItemsArray = JsonUtility.FromJson<SortedItemsArray>(sortingItemsData);
            Items = sortedItemsArray.SortedItems;
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
