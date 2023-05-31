using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

[System.Serializable]
public enum SessionDifficulty { Easy, Normal, Hard }
public enum DictTheme { All, Demons, Gnosticism, Fiction, Angels, Cosmic, Matter, Geometry }

public class GameManager : MonoBehaviour
{
    [SerializeField] public HangmanDictionary _dictionary;
    [SerializeField] private Core hangmanCore;

    public DictTheme selectedTheme;
    public TMP_Dropdown themeDropdown;
    public bool isThemeChosen;
    public static GameManager Instance { get; private set; }
    public BinaryDictionary binaryDictionary;
    public NumberPadManager numpadController;
    public InputManager inputManager;

    [SerializeField] private DisplayManager _displayManager;
    public DisplayManager DisplayManager => _displayManager;
    public GameObject BootSequence;

    [Header("Input Methods")]
    // Input Methods
    public TMPro.TMP_InputField userInput;
    public bool isUsingInput;
    public bool isUsingNumpad;
    public bool isUsingBinary;

    [Header("Logic: Screens")]
    [SerializeField] private GameObject configScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject mainMenuProperties;
    public bool isInConfig;
    public bool isInRunningGame;
    public bool isInMainMenu;

    [Header("Session Difficulty & Lives")]
    public bool isIronmanMode = false;
    public SessionDifficulty difficulty = SessionDifficulty.Normal;
    public SessionDifficulty sessionDifficulty;

    public int maxSessionLives = 3;
    public int minSessionLives = 0;

    [Header("Lexiology Statistics")]
    public int winStreak = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        hangmanCore = FindObjectOfType<Core>();
        _displayManager = FindObjectOfType<DisplayManager>();
        _displayManager.Dictionary = _dictionary;
        BootSequence = GameObject.FindGameObjectWithTag("BootSequence");
    }

    private void Start()
    {
        _displayManager.EndStateScreen.SetActive(false);
    }

    private void Update()
    {
        _displayManager.theme.text = $"Theme: " + GetComponent<GameManager>().selectedTheme;
    }

    private void OnEnable()
    {
        themeDropdown = GameObject.FindGameObjectWithTag("themeDropdown").GetComponent<TMP_Dropdown>();
        PopulateDropDownWithEnum(themeDropdown, selectedTheme);

        configScreen = GameObject.FindGameObjectWithTag("ThemeCanvas");
        gameScreen = GameObject.FindGameObjectWithTag("MainGame");
        mainMenuProperties = GameObject.FindGameObjectWithTag("MMProps");

        StartCoroutine(WaitAllowStartInputs(true));

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            isInMainMenu = true;
            isInConfig = false;
            isInRunningGame = false;
        }
        else if (scene.name == "Config")
        {
            isInMainMenu = false;
            isInConfig = true;
            isInRunningGame = false;
        }
        else if (scene.name == "Main")
        {
            isInMainMenu = false;
            isInConfig = false;
            isInRunningGame = true;
        }

        SwitchCurrentScreens();
    }

    #region ScreenLogic

    private void SwitchCurrentScreens()
    {
        configScreen.SetActive(isInConfig);
        gameScreen.SetActive(isInRunningGame);
        mainMenuProperties.SetActive(isInMainMenu);
    }

    #endregion

    #region ThemeSelection

    public static void PopulateDropDownWithEnum(TMP_Dropdown dropdown, DictTheme targetTheme)
    {
        Type dictThemeType = targetTheme.GetType();
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        foreach (string themeName in Enum.GetNames(dictThemeType))
        {
            newOptions.Add(new TMP_Dropdown.OptionData(themeName));
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(newOptions);
    }

    public void SetSelectedTheme(DictTheme theme)
    {
        selectedTheme = theme;
        isThemeChosen = true;
    }

    public void SetSelectedThemeFromDropDown(TMP_Dropdown selected)
    {
        string selectedOption = selected.options[selected.value].text;
        selectedTheme = (DictTheme)Enum.Parse(typeof(DictTheme), selectedOption);
        isThemeChosen = true;
    }

    #endregion

    #region Buttons

    public void BootToMenu()
    {
        isInMainMenu = true;
        isInConfig = false;
        isInRunningGame = false;
        BootSequence.SetActive(false);
        BootSequence.GetComponent<BootSequence>().MainMenuScreen.SetActive(true);
    }

    public void ToMainMenu()
    {
        _displayManager.WinScreen.SetActive(false);
        _displayManager.EndStateScreen.SetActive(false);

        isInMainMenu = true;
        isInConfig = false;
        isInRunningGame = false;

        SceneManager.LoadScene("Menu");
    }

    public void PressAnything()
    {
        isInMainMenu = true;
        isInConfig = false;
        isInRunningGame = false;

        SceneManager.LoadScene("Menu");
    }

    public void ConfigButton()
    {
        isInMainMenu = false;
        isInConfig = true;
        isInRunningGame = false;

        SceneManager.LoadScene("Config");
    }

    public void StartSession()
    {
        isInMainMenu = false;
        isInConfig = false;
        isInRunningGame = true;

        SceneManager.LoadScene("Main");
        hangmanCore.runningSession = true;
        hangmanCore.GetNewWord();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
        // Save, Then -> Reset All Values Related To Past Session
    }

    public void TryAgain()
    {
        SceneManager.LoadScene("Replay");

        // New Scene Called Replay, Save Current and no NOT REST, start from begging and clear out used words.
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        hangmanCore.runningSession = false;
        Application.Quit();
    }

    public void ClearBinaryInput()
    {
        binaryDictionary.currentBinaryInput = "";
        _displayManager.UpdateGuessesDisplay(hangmanCore.allowedGuesses);
    }

    public void ClearInput()
    {
        if (isUsingInput)
        {
            userInput.text = "";
        }
        else if (isUsingNumpad)
        {
            numpadController.currentInput = "";
        }
        else if (isUsingBinary)
        {
            binaryDictionary.currentBinaryInput = "";
        }
    }

    public void SubmitInput()
    {
        if (isUsingInput)
        {
            string guess = userInput.text;
            if (!string.IsNullOrEmpty(guess))
            {
                hangmanCore.ProcessGuess(guess.ToLower());
                ClearInput();
            }
        }
        else if (isUsingNumpad)
        {
            string guess = numpadController.currentInput;
            if (!string.IsNullOrEmpty(guess))
            {
                hangmanCore.ProcessGuess(guess.ToLower());
                ClearInput();
            }
        }
        else if (isUsingBinary)
        {
            string guess = binaryDictionary.currentBinaryInput;
            if (!string.IsNullOrEmpty(guess))
            {
                hangmanCore.ProcessGuess(guess.ToLower());
                ClearInput();
            }
        }
    }

    #endregion

    #region InputsDomain

    public bool canUpdate = false;

    private IEnumerator WaitAllowStartInputs(bool isInNewScene)
    {
        yield return new WaitForSeconds(5);
        canUpdate = true;
    }

    #endregion
}
