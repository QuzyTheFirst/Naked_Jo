using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelLoader : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
