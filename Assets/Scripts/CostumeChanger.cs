using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostumeChanger : MonoBehaviour
{
    public enum Costumes
    {
        Naked = 0,
        White,
        Black,
    }
    public Costumes CurrentCostume;

    [Header("Costumes")]
    [SerializeField] private Sprite _JONakedSprite;
    [SerializeField] private Sprite _JOWhiteCostumeSprite;
    [SerializeField] private Sprite _JOBlackCostumeSprite;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetCostume(Costumes costume)
    {
        switch (costume)
        {
            case Costumes.Naked:
                _spriteRenderer.sprite = _JONakedSprite;
                break;
            case Costumes.White:
                _spriteRenderer.sprite = _JOWhiteCostumeSprite;
                break;
            case Costumes.Black:
                _spriteRenderer.sprite = _JOBlackCostumeSprite;
                break;

            default:
                _spriteRenderer.sprite = _JONakedSprite;
                break;
        }

        CurrentCostume = costume;
    }

    public void SaveCostume()
    {
        PlayerPrefs.SetInt("Costume", (int)CurrentCostume);
    }

    public void LoadCostume()
    {
        Costumes costume = (Costumes)PlayerPrefs.GetInt("Costume", 0);
        SetCostume(costume);
    }
}
