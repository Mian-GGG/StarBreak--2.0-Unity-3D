using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string _gameScene = "SpaceLevel";
    [SerializeField] private GameObject _settingsPanel;
    private string _lastScene;

    [Header("Buttons")]
    [SerializeField] private GameObject _newGameButton;
    [SerializeField] private GameObject _continueButton;

    [Header("Settings")]
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _sensitivitySlider;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.1f);

        if (_volumeSlider != null)
        {
            _volumeSlider.onValueChanged.RemoveAllListeners();
            _volumeSlider.value = savedVolume;
            _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (_sensitivitySlider != null)
        {
            _sensitivitySlider.onValueChanged.RemoveAllListeners();
            _sensitivitySlider.value = savedSensitivity;
            _sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _lastScene = PlayerPrefs.GetString("LastScene", "");

        bool hasPlayed = PlayerPrefs.HasKey("LastScene");

        //if (_newGameButton != null)
        //    _newGameButton.SetActive(!hasPlayed);

        //if (_continueButton != null)
        //    _continueButton.SetActive(hasPlayed);

        

        AudioListener.volume = savedVolume;

        if (_volumeSlider != null)
        {
            _volumeSlider.onValueChanged.RemoveAllListeners();
            _volumeSlider.value = savedVolume;
            _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (_sensitivitySlider != null)
        {
            _sensitivitySlider.onValueChanged.RemoveAllListeners();
            _sensitivitySlider.value = savedSensitivity;
            _sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
        
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteKey("LastScene");
        PlayerPrefs.Save();
        SceneManager.LoadScene(_gameScene);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(_gameScene);
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        var controls = Object.FindFirstObjectByType<DesktopMovementControls>();
        if (controls != null)
            controls.SetDeadZone(value);
    }

    public void ContinueGame()
    {
        if (!string.IsNullOrEmpty(_lastScene))
            SceneManager.LoadScene(_lastScene);
        else
            SceneManager.LoadScene(_gameScene);
    }

    public void OpenSettings()
    {
        _settingsPanel.SetActive(true);
    }

    public void ResetSettings()
    {
        float defaultVolume = 1f;
        float defaultSensitivity = 0.1f;

        PlayerPrefs.SetFloat("Volume", defaultVolume);
        PlayerPrefs.SetFloat("Sensitivity", defaultSensitivity);
        PlayerPrefs.Save();

        if (_volumeSlider != null)
            _volumeSlider.value = defaultVolume;

        if (_sensitivitySlider != null)
            _sensitivitySlider.value = defaultSensitivity;

        AudioListener.volume = defaultVolume;

        var controls = FindFirstObjectByType<DesktopMovementControls>();
        if (controls != null)
            controls.SetDeadZone(defaultSensitivity);
    }

    public void CloseSettings()
    {
        _settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}