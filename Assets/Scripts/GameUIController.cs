using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIController : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] private Sprite _goldenKey;
    [SerializeField] private Sprite _silverKey;
    [SerializeField] private Sprite _bronzeKey;
    [SerializeField] private Image[] _keysImages;

    [Header("Level End Pointer")]
    [SerializeField] private RectTransform _pointerRectTransform;
    [SerializeField] private Image _pointerImage;
    [SerializeField] private Sprite _arrowSprite;
    [SerializeField] private Sprite _destinationSprite;
    private Transform _beginTransform;
    private Transform _endTransform;
    private bool _isPointerActivated;

    private float _currentKeyCount;

    private bool _updatePossession = true;
    private bool _updateRolling = true;
    private bool _updateExplosion = true;

    [Header("UI Elements")]
    [SerializeField] private Color _readyToUseColor;
    [SerializeField] private Color _turnedOffColor;

    [Header("Ammo")]
    [SerializeField] private GameObject _ammoGO;
    [SerializeField] private TextMeshProUGUI _ammoAmount;

    [Header("Possession")]
    [SerializeField] private Color _possessionColor;
    [SerializeField] private Color _possessionCooldownColor;
    [SerializeField] private Image _possessionBG;
    [SerializeField] private Image _possessionIcon;

    [Header("Roll")]
    [SerializeField] private Color _rollingColor;
    [SerializeField] private Color _rollingCooldownColor;
    [SerializeField] private Image _rollingBG;
    [SerializeField] private Image _rollingIcon;

    [Header("Explosion")]
    [SerializeField] private Color _explosionColor;
    [SerializeField] private Color _explosionCooldownColor;
    [SerializeField] private Image _explosionBG;
    [SerializeField] private Image _explosionIcon;

    [Header("Now Kill Yourself")]
    [SerializeField] private GameObject _nowKillYourselfGO;

    public static GameUIController Instance;

    private void Awake()
    {
        Instance = this;

        HideAllKeys();

        _pointerRectTransform.gameObject.SetActive(false);
        _isPointerActivated = false;

        _ammoGO.SetActive(false);
    }

    private void Update()
    {
        if (_isPointerActivated)
        {
            Vector2 toPosition = _endTransform.position;
            Vector2 fromPosition = _beginTransform.position;
            Vector2 dir = (toPosition - fromPosition).normalized;

            float borderSize = 100f;
            Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(_endTransform.position);
            bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

            if (isOffScreen)
            {
                _pointerImage.sprite = _arrowSprite;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                _pointerRectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
                _pointerImage.SetNativeSize();
                Vector2 cappedTargetScreenPosition = targetPositionScreenPoint;

                if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
                if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
                if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
                if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;

                _pointerRectTransform.position = cappedTargetScreenPosition;
                _pointerRectTransform.localPosition = new Vector2(_pointerRectTransform.localPosition.x, _pointerRectTransform.localPosition.y);
            }
            else
            {
                _pointerRectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
                _pointerImage.sprite = _destinationSprite;
                _pointerImage.SetNativeSize();
                _pointerRectTransform.position = targetPositionScreenPoint;
                _pointerRectTransform.localPosition = new Vector2(_pointerRectTransform.localPosition.x, _pointerRectTransform.localPosition.y);
            }
        }
    }

    public void SetKeys(List<Key.KeyType> keys)
    {
        HideAllKeys();

        if(_currentKeyCount < keys.Count)
        {
            SoundManager.Instance.Play("KeyPickUp");
        }

        for(int i = 0; i < keys.Count; i++)
        {
            if (i == 5)
                break;

            _keysImages[i].color = Color.white;
            _keysImages[i].sprite = GetKeySrite(keys[i]);
        }

        _currentKeyCount = keys.Count;
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

    public void ActivateLevelEndPointer(Transform beginPoint, Transform endPoint)
    {
        _beginTransform = beginPoint;
        _endTransform = endPoint;
        _pointerRectTransform.gameObject.SetActive(true);
        _isPointerActivated = true;
    }

    public void SetPossessionValue(float value)
    {
        if (!_updatePossession)
            return;

        _possessionBG.fillAmount = value;

        _possessionBG.color = value == 1 ? _readyToUseColor : _possessionCooldownColor;
    }

    public void TogglePossession(bool value)
    {
        if(value == true)
        {
            _possessionIcon.color = _possessionColor;
        }
        else
        {
            _possessionBG.fillAmount = 1;
            _possessionBG.color = _turnedOffColor;
            _possessionIcon.color = _possessionColor * _turnedOffColor;
        }

        _updatePossession = value;
    }

    public void SetRollingValue(float value)
    {
        if (!_updateRolling)
            return;

        _rollingBG.fillAmount = value;

        _rollingBG.color = value == 1 ? _readyToUseColor : _rollingCooldownColor;
    }

    public void ToggleRolling(bool value)
    {
        if (value == true)
        {
            _rollingIcon.color = _rollingColor;
        }
        else
        {
            _rollingBG.fillAmount = 1;
            _rollingBG.color = _turnedOffColor;
            _rollingIcon.color = _rollingColor * _turnedOffColor;
        }

        _updateRolling = value;
    }

    public void SetExplosionValue(float value)
    {
        if (!_updateExplosion)
            return;

        _explosionBG.fillAmount = value;

        _explosionBG.color = value == 1 ? _readyToUseColor : _explosionCooldownColor;
    }

    public void ToggleExplosion(bool value)
    {
        if (value == true)
        {
            _explosionIcon.color = _rollingColor;
        }
        else
        {
            _explosionBG.fillAmount = 1;
            _explosionBG.color = _turnedOffColor;
            _explosionIcon.color = _explosionColor * _turnedOffColor;
        }

        _updateExplosion = value;
    }

    public void SetAmmoAmount(int value)
    {
        if (!_ammoGO.activeInHierarchy)
            _ammoGO.SetActive(true);

        _ammoAmount.text = $"{value}rnds";
    }

    public void ToggleNowKillYourself(bool value)
    {
        _nowKillYourselfGO.SetActive(value);
    }

    public void DisableAmmoAmount()
    {
        _ammoGO.SetActive(false);
    }
}
