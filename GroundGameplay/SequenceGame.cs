using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SequenceGame : MonoBehaviour
{
    public static SequenceGame Instance;

    [Header("UI")]
    public GameObject panel, crosshair;
    public TextMeshProUGUI statusText;

    [Header("Buttons")]
    public Button[] colorButtons; 
    public Color[] activeColors;  
    public Color[] dimColors;    

    [Header("Settings")]
    public float flashDuration = 0.5f;
    public float flashGap = 0.2f;
    public int startingLength = 3, numberOfLevels = 4;

    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();
    private bool acceptingInput = false;
    private int currentLevel = 0;

    public System.Action onSuccess;
    public System.Action onFail;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        panel.SetActive(false); 
    }

    void Update()
    {
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            CloseGame();
        }
    }

    public void StartGame(System.Action successCallback, System.Action failCallback)
    {
        Time.timeScale = 0;
        crosshair.SetActive(false);
        onSuccess = successCallback;
        onFail = failCallback;

        sequence.Clear();
        playerInput.Clear();
        currentLevel = 0;

        for (int i = 0; i < startingLength; i++)
            sequence.Add(Random.Range(0, colorButtons.Length));

        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        for (int i = 0; i < colorButtons.Length; i++)
        {
            int index = i;
            colorButtons[i].onClick.RemoveAllListeners();
            colorButtons[i].onClick.AddListener(() => OnPlayerPressButton(index));
        }

        StartCoroutine(NextRound());
    }

    void CloseGame()
    {
        panel.SetActive(false);
        crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    IEnumerator NextRound()
    {
        acceptingInput = false;
        currentLevel++;
        statusText.text = "Watch carefully...";

        if (currentLevel > 1)
            sequence.Add(Random.Range(0, colorButtons.Length));
        Debug.Log("Sequence length is now: " + sequence.Count);
        yield return new WaitForSecondsRealtime(0.8f);

        foreach (int index in sequence)
        {
            yield return StartCoroutine(FlashButton(index));
            yield return new WaitForSecondsRealtime(flashGap);
        }

        playerInput.Clear();
        acceptingInput = true;
        statusText.text = "Your turn! Repeat the sequence.";
    }

    IEnumerator FlashButton(int index)
    {
        SetButtonColor(index, activeColors[index]);
        yield return new WaitForSecondsRealtime(flashDuration);
        SetButtonColor(index, dimColors[index]);
    }

    void SetButtonColor(int index, Color color)
    {
        color.a = 1f;
        colorButtons[index].GetComponent<Image>().color = color;
    }

    void OnPlayerPressButton(int index)
    {
        if (!acceptingInput) return;

        playerInput.Add(index);
        StartCoroutine(FlashButton(index));

        int step = playerInput.Count - 1;

        if (playerInput[step] != sequence[step])
        {
            StartCoroutine(HandleFail());
            return;
        }

        if (playerInput.Count == sequence.Count)
        {
            acceptingInput = false;
            StartCoroutine(HandleSuccess());
        }
    }

    IEnumerator HandleSuccess()
    {
        statusText.text = "Correct!";
        yield return new WaitForSecondsRealtime(1f);

        if (currentLevel >= numberOfLevels)
        {
            CloseGame();
            onSuccess?.Invoke();
        }
        else
        {
            StartCoroutine(NextRound());
        }
    }

    IEnumerator HandleFail()
    {
        acceptingInput = false;
        statusText.text = "Wrong! Try again...";

        foreach (Button btn in colorButtons)
        {
            btn.GetComponent<Image>().color = Color.red;
        }

        yield return new WaitForSecondsRealtime(1f);

        for (int i = 0; i < colorButtons.Length; i++)
            SetButtonColor(i, dimColors[i]);

        StartGame(onSuccess, onFail);
    }
}
