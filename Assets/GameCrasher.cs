using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCrasher : MonoBehaviour
{
    private void Awake()
    {
        Application.Quit();
    }
}
