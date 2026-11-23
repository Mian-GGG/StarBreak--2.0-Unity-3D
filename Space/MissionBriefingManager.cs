using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MissionBriefingManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _briefingPanel;
    [SerializeField] private TMP_Text _briefingText;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _acceptButton;

    [Header("Scene")]
    [SerializeField] private string _gameScene = "SpaceLevel";

    private int _currentPage = 0;

    private string[] _pages = {
    "Commander Riven Dale. We've lost contact with our science team.\n\nFive of Earth's brightest minds were conducting a classified research mission in the Solaris system — a newly discovered solar system far beyond our own. Three days ago, their signal went dark.",
    "The Xyrath Collective has moved into the system. They've captured our scientists and seized control of every planet. We don't know what they want with them — and we can't afford to find out.",
    "You're the closest asset we have, Commander. But your ship took heavy damage warping through space. You're running at half capacity.\n\nEach planet has a redundant base left behind by the science team. Scavenge the parts. Fix your ship. Fight your way through.",
    "The scientists are alone out there, Commander.\n\nYou're their only chance."
};

    public void StartBriefing()
    {
        _currentPage = 0;
        _mainMenuPanel.SetActive(false);
        _briefingPanel.SetActive(true);
        ShowPage(_currentPage);
    }

    public void NextPage()
    {
        _currentPage++;

        if (_currentPage >= _pages.Length)
            return;

        ShowPage(_currentPage);
    }

    void ShowPage(int index)
    {
        _briefingText.text = _pages[index];

        bool isLastPage = index >= _pages.Length - 1;
        _nextButton.SetActive(!isLastPage);
        _acceptButton.SetActive(isLastPage);
    }

    public void AcceptMission()
    {
        PlayerPrefs.DeleteKey("LastScene");
        PlayerPrefs.Save();
        SceneManager.LoadScene(_gameScene);
    }
}