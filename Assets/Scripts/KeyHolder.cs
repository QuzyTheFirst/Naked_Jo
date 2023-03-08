using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] private Transform _goldKeyPf;
    [SerializeField] private Transform _silverKeyPf;
    [SerializeField] private Transform _bronzeKeyPf;

    public List<Key.KeyType> KeyList;

    public void AddKey(Key.KeyType keyType) => KeyList.Add(keyType);
    

    public void RemoveKey(Key.KeyType keyType) => KeyList.Remove(keyType);
    

    public bool ContainsKey(Key.KeyType keyType) => KeyList.Contains(keyType);

    // AI
    public void DropAllKeys()
    {
        foreach(Key.KeyType keyType in KeyList)
        {
            switch (keyType)
            {
                case Key.KeyType.Gold:
                    Instantiate(_goldKeyPf, transform.position, Quaternion.identity);
                    break;
                case Key.KeyType.Silver:
                    Instantiate(_silverKeyPf, transform.position, Quaternion.identity);
                    break;
                case Key.KeyType.Bronze:
                    Instantiate(_bronzeKeyPf, transform.position, Quaternion.identity);
                    break;
            }
        }

        ClearKeyList();
    }

    public void ClearKeyList() => KeyList.Clear();

}
