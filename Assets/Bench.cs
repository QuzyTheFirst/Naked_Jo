using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bench : MonoBehaviour, IInteractable
{
    [Header("Camera")]
    [SerializeField] private Transform _cameraTarget;

    [Header("Sprites")]
    [SerializeField] private Sprite _bench;
    [SerializeField] private Sprite _joOnBench;

    [Header("Keyboard button")]
    [SerializeField] private GameObject _keyboardButton;

    private SpriteRenderer _spriteRenderer;

    private UnitsHandler _unitsHandler;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool Interaction(UnitsHandler unitsHandler)
    {
        _spriteRenderer.sprite = _joOnBench;

        unitsHandler.ToggleCurrentUnitVisibility(false);
        unitsHandler.SetCameraTarget(_cameraTarget.position);
        unitsHandler.ToggleInterfaceVisibility(false);

        _keyboardButton.SetActive(false);

        StartCoroutine(InteractionUdpate(unitsHandler));

        return true;
    }

    IEnumerator InteractionUdpate(UnitsHandler unitsHandler)
    {
        while (true)
        {
            if (unitsHandler.IsCurrentUnitMoving())
            {
                StopInteraction(unitsHandler);
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void StopInteraction(UnitsHandler unitsHandler)
    {
        _spriteRenderer.sprite = _bench;

        unitsHandler.ToggleCurrentUnitVisibility(true);
        unitsHandler.ResetCameraTarget();
        unitsHandler.ToggleInterfaceVisibility(true);

        _keyboardButton.SetActive(true);
    }
}
