using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Sprite _goldenKey;
    [SerializeField] private Sprite _silverKey;
    [SerializeField] private Sprite _bronzeKey;
    [SerializeField] private Image[] _keysImages;

    public static GameUIController  Instance;

    private void Awake()
    {
        Instance = this;

        HideAllKeys();
    }

    public void SetKeys(List<Key.KeyType> keys)
    {
        HideAllKeys();

        for(int i = 0; i < keys.Count; i++)
        {
            if (i == 5)
                break;

            _keysImages[i].color = Color.white;
            _keysImages[i].sprite = GetKeySrite(keys[i]);
        }
    }

    private void HideAllKeys()
    {
        foreach(Image image in _keysImages)
        {
            image.color = Color.clear;
        }
    }

    private Sprite GetKeySrite(Key.KeyType keyType)
    {
        switch (keyType)
        {
            case Key.KeyType.Gold:
                return _goldenKey;

            case Key.KeyType.Silver:
                return _silverKey;

            case Key.KeyType.Bronze:
                return _bronzeKey;

            default:
                return null;
        }
    }
}
