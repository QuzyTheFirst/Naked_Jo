using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private GameObject _levelsMenu;

    [Header("Settings")]
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TextMeshProUGUI _volumeText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ToggleSettingsMenu(bool value)
    {
        _settingsMenu.SetActive(value);
    }

    public void ToggleLevelsMenu(bool value)
    {
        _levelsMenu.SetActive(value);
    }

    private void OnEnable()
    {
        _volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1);
        _volumeText.text = $"Volume: {(int)(_volumeSlider.value * 100)}%";

        _volumeSlider.onValueChanged.AddListener((value) =>
        {
            _volumeText.text = $"Volume: {(int)(value * 100)}%";
            SoundManager.Instance.ChangeSoundManagerVolume(value);
            PlayerPrefs.SetFloat("Volume", _volumeSlider.value);
        });
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    private void OnDisable()
    {
        _volumeSlider.onValueChanged.RemoveAllListeners();
    }
}
