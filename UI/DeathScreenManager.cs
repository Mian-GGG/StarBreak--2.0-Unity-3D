using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem _lightningEffect;
    [SerializeField] private ParticleSystem _smokeEffect;
    [SerializeField] private TMP_Text _deathMessageText;
    [SerializeField] private bool _isGround;

    string[] _deathMessages = {
    "The galaxy wasn't ready for you",
    "Your ship has been reduced to space dust",
    "The enemy prevails... for now",
    "Systems critical � pilot lost",
    "You fought bravely, pilot",
    "The stars claimed another soul"
};

    string[] _deathMessagesGround = {
    "The mission has failed, Commander",
    "Soldiers don't die, they reload",
    "The enemy has claimed this ground",
    "Your unit has been compromised",
    "No reinforcements incoming",
    "The battlefield belongs to them now",
    "Even the best fall sometimes, Commander",
    "Retreat was not an option... neither was survival",
    "The planet remains under enemy control",
    "Your signal has gone dark",
    "Command has lost contact with the unit",
    "Another hero falls on alien soil",
    "The enemy advances � regroup and try again",
    "Casualties reported at your location",
    "Mission critical failure � retry?"
};

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;

        if (_lightningEffect != null) _lightningEffect.Play();
        if (_isGround)
        {
            _deathMessageText.text =
            _deathMessagesGround[Random.Range(0, _deathMessagesGround.Length)];
        }
        else
        {
            _deathMessageText.text =
            _deathMessages[Random.Range(0, _deathMessages.Length)];
        }
     
    }

    public void RestartLevel()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", "SpaceLevel");
        SceneManager.LoadScene(lastScene);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
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
