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
    [SerializeField] HangmanDictionary _dictionary;

    public DictTheme selectedTheme;
    public TMP_Dropdown themeDropdown;
    public bool isThemeChosen;
    public static GameManager Instance { get; private set; }
    public BinaryDictionary binaryDictionary;
    public NumberPadManager numpadController;
    public InputManager inputManager;

    private List<int> _matchIndices = new List<int>();
    public List<int> MatchIndices
    {
        get => _matchIndices;
        set => _matchIndices = value;
    }
    public int IRONMAN_MODE_EASY_GUESSES { get; private set; } = 20;
    public int IRONMAN_MODE_MEDIUM_GUESSES { get; private set; } = 16;
    public int IRONMAN_MODE_HARD_GUESSES { get; private set; } = 6;

    [Header("Input Methods")]
    // Input Methods
    public TMPro.TMP_InputField userInput;
    public bool isUsingInput;
    public bool isUsingNumpad;
    public bool isUsingBinary;

    [Header("Word")]
    [SerializeField] string currentString;
    [SerializeField] string getScene;
    private string currentWord;
    [SerializeField] string _currentWord;
    [SerializeField] string currentGuess = "";


    [Header("Session Difficulty & Lives")]
    public SessionDifficulty difficulty = SessionDifficulty.Normal;
    public SessionDifficulty sessionDifficulty;

    public int maxSessionLives = 3;
    public int minSessionLives = 0;
    public int currentSessionLives;


    [Header("Standard Game Logic Variables")]
    public bool runningSession;
    public bool isIronmanMode = false;
    public int wordsPerExtraLife = 3;
    public bool isProcessingGuess = false;
    public bool failWordCompletion = false;

    public int allowedGuesses = 6;
    public int initialGuessAmount;

    public bool wordCompleted = false;
    public int winStreak = 0;

    [SerializeField] private float maxSolveTime = 3f; // in Minutes
    public float remainingTimeToSolve;

    [SerializeField] private int numberOfWordsSolved;

    private DisplayManager _displayManager;

    [Header("Logic: Lists")]
    [SerializeField] public List<string> solvedWords = new List<string>();
    [SerializeField] private List<string> usedWords = new List<string>();

    [SerializeField] private bool isTimeRemainingCodeRunning = false;

    [Header("Logic: Screens")]
    [SerializeField] GameObject ConfigScreen;
    [SerializeField] GameObject GameScreen;
    [SerializeField] GameObject MainMenuProperties;
    public bool isInConfig;
    [SerializeField] public bool isInRunningGame;
    [SerializeField] public bool isInMainMenu;



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
    }

    private void Update()
    {
        _displayManager.UpdateWordDisplay(_currentWord, isIronmanMode, sessionDifficulty);

        if (canUpdate)
        {
            if (runningSession)
            {
                isTimeRemainingCodeRunning = true; // set to true when the code block runs

                if (isUsingInput)
                {
                    inputManager.SetInputActive();
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SubmitGuess();
                    }
                }
                else if (isUsingNumpad)
                {
                    inputManager.SetNumpadActive();
                    string guess = numpadController.currentInput;
                }
                else if (isUsingBinary)
                {
                    inputManager.SetBinaryActive();
                    string guess = binaryDictionary.currentBinaryInput;
                    char matchKey = (char)binaryDictionary.GetMatchedKey(guess);
                    if (matchKey != default)
                    {
                         ProcessGuess(matchKey.ToString());
                         binaryDictionary.currentBinaryInput = "";
                    }
                }

                if (allowedGuesses == 0)
                {
                    if (isIronmanMode)
                    {
                        wordsPerExtraLife--;
                        if (wordsPerExtraLife == 0)
                        {
                            wordsPerExtraLife = 3;
                            maxSessionLives++;
                        }
                    }
                    else
                    {
                        currentSessionLives--;
                    }

                    if (currentSessionLives < minSessionLives)
                    {
                        EndSession(false);
                    }
                }
            }
            if (isTimeRemainingCodeRunning && remainingTimeToSolve <= 0)
            {
                isTimeRemainingCodeRunning = false; // set back to false
            }
            _displayManager.UpdateGuessesDisplay(allowedGuesses);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (_displayManager.sceneName == "Menu")
        {
            SwitchCurrentScreens();
        }
        else if (_displayManager.sceneName == "Config")
        {
            SwitchCurrentScreens();
        }
        else if (_displayManager.sceneName == "Main")
        {
            SwitchCurrentScreens();
        }
        else { }
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

    #region WordSelection
    private string GetRandomWord()
    {
        string[] possibleWords;

        if (selectedTheme == DictTheme.All)
        {
            possibleWords = _dictionary.GetAllWords();
        }
        else
        {
            possibleWords = _dictionary.GetWordsByTheme(selectedTheme.ToString());
        }

        possibleWords = possibleWords.Except(solvedWords).ToArray();
        possibleWords = possibleWords.Except(usedWords).ToArray();

        string word;

        if (possibleWords.Length > 0)
        {
            word = possibleWords[Random.Range(0, possibleWords.Length)];
        }
        else
        {
            usedWords.Clear();
            solvedWords.Clear();
            word = GetRandomWord();
        }

        return word;
    }
    public void GetNewWord()
    {
        if (wordCompleted || failWordCompletion)
        {
            _displayManager.WinScreen.SetActive(false);
            _displayManager.LoseScreen.SetActive(false);
            wordCompleted = false;
            failWordCompletion = false;
        }

        if (!isProcessingGuess)
        {
            currentWord = GetRandomWord();
            _currentWord = currentWord; // set the _currentWord field to the current word
            remainingTimeToSolve = maxSolveTime * 60f;
            initialGuessAmount = allowedGuesses;

            if (isIronmanMode)
            {
                if (sessionDifficulty == SessionDifficulty.Easy)
                {
                    initialGuessAmount = IRONMAN_MODE_EASY_GUESSES;
                }
                else if (sessionDifficulty == SessionDifficulty.Normal)
                {
                    initialGuessAmount = IRONMAN_MODE_MEDIUM_GUESSES;
                }
                else if (sessionDifficulty == SessionDifficulty.Hard)
                {
                    initialGuessAmount = IRONMAN_MODE_HARD_GUESSES;
                }
            }

            _displayManager.UpdateWordDisplay(currentWord, isIronmanMode, sessionDifficulty);
            StartCoroutine(CountdownTimerCoroutine());
            isProcessingGuess = true;
        }
    }

    public void ProcessGuess(string guess)
    {
        guess.ToLower();
        currentString = "";
        if (Regex.IsMatch(guess, "^[a-zA-Z]$"))
        {
            bool foundMatch = false;
            for (int i = 0; i < currentWord.Length; i++)
            {
                if (currentWord[i].ToString().ToLower() == guess.ToLower())
                {
                    _displayManager.UpdateLetterDisplay(i, guess);
                    foundMatch = true;
                    currentString += guess.ToLower(); // add the guessed letter to the current string
                }
            }

            if (foundMatch)
            {
                if (_displayManager.WordIsComplete())
                {
                    Debug.Log("Match found, in ProcessGuess(guess), part of if(foundMatch)");
                    solvedWords.Add(currentWord);
                    numberOfWordsSolved++;

                    if (numberOfWordsSolved % wordsPerExtraLife == 0 && isIronmanMode)
                    {
                        maxSessionLives++;
                    }

                    if (maxSessionLives > 5)
                    {
                        maxSessionLives = 5;
                    }

                    if (numberOfWordsSolved == 5 && isIronmanMode)
                    {
                        allowedGuesses++;
                    }

                    if (solvedWords.Count == _dictionary.GetAllWords().Length)
                    {
                        EndSession(true);
                    }
                    else
                    {
                        GetNewWord();
                        _displayManager.UpdateWordDisplay(currentWord, isIronmanMode, sessionDifficulty);
                    }
                }
            }
            else
            {
                allowedGuesses--;
                _displayManager.UpdateGuessesDisplay(allowedGuesses);

                if (allowedGuesses == 0)
                {
                    currentSessionLives--;
                    if (isIronmanMode)
                    {
                        wordsPerExtraLife--;
                        if (wordsPerExtraLife == 0)
                        {
                            wordsPerExtraLife = 3;
                            maxSessionLives++;
                        }
                    }

                    if (currentSessionLives < minSessionLives)
                    {
                        EndSession(false);
                    }
                    else
                    {
                        GetNewWord();
                    }
                }
            }
            UpdateGuessString(guess);
        }
        else
        {
            Debug.LogWarning("Invalid guess: " + guess);
            isProcessingGuess = false;
        }
    }

    public void UpdateGuessString(string guess)
    {
        if (isUsingBinary)
        {
            if (binaryDictionary.currentBinaryInput.Length < BinaryDictionary.maxBinaryDigitCount)
            {
                binaryDictionary.currentBinaryInput += guess.ToLower();
                _displayManager.UpdateGuessesDisplay(allowedGuesses);
            }
        }
        else
        {
            string guessString = string.IsNullOrEmpty(guess) ? "" : guess.ToLower();
            string currentWordUpper = currentWord.ToLower();

            for (int i = 0; i < currentWordUpper.Length; i++)
            {
                if (currentWordUpper[i] == guessString[0])
                {
                    _displayManager.UpdateLetterDisplay(i, guessString);
                }
            }

            if (isUsingNumpad)
            {
                numpadController.currentInput += guessString;
            }
            else if (isUsingInput)
            {
                if (userInput.text.Length < 1)
                {
                    userInput.text += guessString;
                }
            }
        }
    }
    public void SubmitGuess()
    {
        string guess = userInput.text;
        if (!string.IsNullOrEmpty(guess))
        {
            ProcessGuess(guess);
            ClearInput();
        }
    }

    public void EndEditGuess(string guess)
    {
        if (!string.IsNullOrEmpty(guess))
        {
            ProcessGuess(guess);
        }
    }

    #endregion

    public void SolvedWordSuccessfully()
    { 
        if (runningSession && wordCompleted)
        {
            winStreak++;
        }
    }

    private void EndSession(bool victory)
    {
        runningSession = false;
        Debug.Log("End Session called");

        if (CountdownTimerCoroutine() != null)
        {
            StopCoroutine(CountdownTimerCoroutine());
        }

        if (victory)
        {
            _displayManager.DisplayVictory();
        }
        else
        {
            _displayManager.DisplayDefeat();
        }

        //currentWord = "";
        //solvedWords.Clear();
        //usedWords.Clear();
        //numberOfWordsSolved = 0;
        //allowedGuesses = 6;
        //currentSessionLives = maxSessionLives;
        //maxSessionLives = 3;
    }
    private IEnumerator CountdownTimerCoroutine()
    {
        while (remainingTimeToSolve > 0f)
        {
            remainingTimeToSolve -= Time.deltaTime;

            yield return null;
        }

        EndSession(false);
    }
    private IEnumerator UpdateDisplayCoroutine()
    {
        while (remainingTimeToSolve > 0)
        {
            _displayManager.UpdateGuessesDisplay(allowedGuesses);
            _displayManager.UpdateTimeDisplay(remainingTimeToSolve);
            yield return new WaitForSeconds(1f);
        }
    }


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
        runningSession = true;
        GetNewWord();
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
        runningSession = false;
        Application.Quit();
    }

    public void ClearBinaryInput()
    {
        binaryDictionary.currentBinaryInput = "";
        _displayManager.UpdateGuessesDisplay(allowedGuesses);
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
                ProcessGuess(guess.ToLower());
                ClearInput();
            }
        }
        else if (isUsingNumpad)
        {
            string guess = numpadController.currentInput;
            if (!string.IsNullOrEmpty(guess))
            {
                ProcessGuess(guess.ToLower());
                ClearInput();
            }
        }
        else if (isUsingBinary)
        {
            string guess = binaryDictionary.currentBinaryInput;
            if (!string.IsNullOrEmpty(guess))
            {
                ProcessGuess(guess.ToLower());
                ClearInput();
            }
        }
    }

    #endregion

    #region InputsDomain
    private bool newScene;
    private bool canUpdate = false;
    private IEnumerator WaitAllowStartInputs(bool isInNewScene)
    {
        yield return new WaitForSeconds(5);
        canUpdate = true;
    }

    #endregion

}