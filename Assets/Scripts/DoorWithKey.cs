using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorWithKey : Door
{
    [SerializeField] private Key.KeyType _keyType;
    public Key.KeyType KeyType { get { return _keyType; } }

    private new void Awake()
    {
        base.Awake();
    }
}
