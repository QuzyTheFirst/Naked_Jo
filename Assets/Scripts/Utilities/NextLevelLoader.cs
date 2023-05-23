using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelLoader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _graphics;
    [SerializeField] private Door _nextLevelLoaderDoor;
    [SerializeField] private Door.OpenDoorDirection _openDoorDirection;

    private BoxCollider2D _boxCollider;
    private bool _isActivated;

    public SpriteRenderer Graphics { get { return _graphics; } set { _graphics = value; } }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Activate()
    {
        if (_isActivated)
            return;

        _boxCollider.enabled = true;
        _nextLevelLoaderDoor.OpenDoor(_openDoorDirection);
        _isActivated = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            collision.GetComponent<Unit>().MyCostumeChanger.SaveCostume();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
