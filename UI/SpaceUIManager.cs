using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SpaceUIManager : MonoBehaviour
{
    public static SpaceUIManager Instance;

    [Header("Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _gameHUD;
    [SerializeField] private GameObject _settingPanel;

    [Header("Settings")]
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _sensitivitySlider;

    [Header("Scene Names")]
    [SerializeField] private string _mainMenuScene = "MainMenu";
    [SerializeField] private string _landingScene = "LandingScene";

    private bool _isPaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
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

        AudioListener.volume = savedVolume;
        var controls = FindObjectOfType<DesktopMovementControls>();
        if (controls != null)
            controls.SetDeadZone(savedSensitivity);
        
        ResumeGame();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
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
        _settingPanel.SetActive(false);
    }
    // ===================== PAUSE =====================

    public void TogglePause()
    {
        if (_isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        _pausePanel.SetActive(true);
        _gameHUD.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        if (_pausePanel != null) _pausePanel.SetActive(false);
        if(_settingPanel != null) _settingPanel.SetActive(false);
        if (_gameHUD != null) _gameHUD.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // ===================== LANDING =====================

    public void TriggerLanding()
    {
        SceneManager.LoadScene(_landingScene);
    }

    // ===================== BUTTONS =====================

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_mainMenuScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ===================== SETTINGS =====================

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

    public void OpenSettings()
    {
        _settingPanel.SetActive(true);
    }
}