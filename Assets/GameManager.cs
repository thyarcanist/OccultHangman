using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;

[System.Serializable]
public enum SessionDifficulty { Easy, Normal, Hard }
public enum DictTheme { All, Demons, Gnosticism, Fiction, Angels, Cosmic, Matter, Geometry }
public class GameManager : MonoBehaviour
{
    [SerializeField] public HangmanDictionary _dictionary;
    [SerializeField] Core hangmanCore;

    public DictTheme selectedTheme;
    public TMP_Dropdown themeDropdown;
    public bool isThemeChosen;
    public static GameManager Instance { get; private set; }
    public BinaryDictionary binaryDictionary;
    public NumberPadManager numpadController;
    public InputManager inputManager;


    [Header("Input Methods")]
    // Input Methods
    public TMPro.TMP_InputField userInput;
    public bool isUsingInput;
    public bool isUsingNumpad;
    public bool isUsingBinary;



    [SerializeField] private DisplayManager _displayManager;
    [SerializeField] public DisplayManager DisplayManager => _displayManager;


    [Header("Logic: Screens")]
    [SerializeField] GameObject ConfigScreen;
    [SerializeField] GameObject GameScreen;
    [SerializeField] GameObject MainMenuProperties;
    public bool isInConfig;
    [SerializeField] public bool isInRunningGame;
    [SerializeField] public bool isInMainMenu;


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
        _displayManager = GameObject.FindWithTag("DisplayManager").GetComponent<DisplayManager>();
        _displayManager.Dictionary = _dictionary;

    }

    private void Start()
    {
        _displayManager.GetComponent<DisplayManager>().EndStateScreen.SetActive(false);
    }

    private void OnEnable()
    {
        themeDropdown = GameObject.FindGameObjectWithTag("themeDropdown").GetComponent<TMP_Dropdown>();
        PopulateDropDownWithEnum(themeDropdown, selectedTheme);
        Debug.Log("themeDropdown is null: " + (themeDropdown == null));

        ConfigScreen = GameObject.FindGameObjectWithTag("ThemeCanvas");
        GameScreen = GameObject.FindGameObjectWithTag("MainGame");
        MainMenuProperties = GameObject.FindGameObjectWithTag("MMProps");

        StartCoroutine(WaitAllowStartInputs(true));

        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the sceneLoaded event
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

        SwitchCurrentScreens(); // Call the screen switching logic
    }


    #region ScreenLogic

    public void SwitchCurrentScreens()
    {
        if (isInConfig)
        {
            ConfigScreen.SetActive(true);
            GameScreen.SetActive(false);
            MainMenuProperties.SetActive(false);
        }
        else if (isInRunningGame)
        {
            ConfigScreen.SetActive(false);
            GameScreen.SetActive(true);
            MainMenuProperties.SetActive(false);
        }
        else if (isInMainMenu)
        {
            ConfigScreen.SetActive(false);
            GameScreen.SetActive(false);
            MainMenuProperties.SetActive(true);
        }
    }

    #endregion

    #region ThemeSelection
    public static void PopulateDropDownWithEnum(TMP_Dropdown dropdown, DictTheme targetTheme)
    {
        Type DictTheme = targetTheme.GetType();
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < Enum.GetNames(DictTheme).Length; i++)
        {
            newOptions.Add(new TMP_Dropdown.OptionData(Enum.GetName(DictTheme, i)));
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

    public void ToMainMenu()
    {
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
    private bool newScene;
    public bool canUpdate = false;
    private IEnumerator WaitAllowStartInputs(bool isInNewScene)
    {
        yield return new WaitForSeconds(5);
        canUpdate = true;
    }

    #endregion

}
