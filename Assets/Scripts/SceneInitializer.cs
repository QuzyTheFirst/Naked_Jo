using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private Transform _soundManagerPf;

    private void Start()
    {
        if(SoundManager.Instance == null)
        {
            SoundManager.CreateInstance(_soundManagerPf);
        }
    }
}
