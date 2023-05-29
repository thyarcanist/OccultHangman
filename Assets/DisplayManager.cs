using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayManager : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    [SerializeField] private Core hangmanCore;
    public static DisplayManager Instance { get; private set; }
    public HangmanDictionary Dictionary { get; set; }
    private SessionDifficulty currentSession;

    public bool showDashes = true;
    public bool isIronOn = false;
    public int _modifiedLives;
    public int _guessesLeft;
    public int getWinStreak;
    public float _maxTimeRemaining;

    public string scrnMsg;
    public string sceneName;
    public string chosenTheme;
    private string _currentWord;

    public TMP_Text theme;
    public GameObject DisplayButtons;
    public GameObject ThemeDiffButtons;
    public GameObject GameButtons;

    private bool _isEndStateDisplayed = false;
    private bool isWordCompleted = false;
    private bool isCoreRunning = false;

    public GameObject EndStateScreen;
    public GameObject WinScreen;
    public GameObject LoseScreen;

    private List<int> _matchIndices = new List<int>();
    public List<string> guessedLetters = new List<string>();
    public GameObject letterBank;

    public TMP_Text wordDisplay = null;
    [SerializeField] public TMP_Text timeRemainingDisplay = null;

    bool isComplete;

    private void Awake()
    {
        // Set the Dictionary reference first
        Dictionary = FindObjectOfType<HangmanDictionary>();
        if (Dictionary == null)
        {
            Debug.LogError("HangmanDictionary component not found!");
        }
        else
        {
            Debug.Log("HangmanDictionary component found!");
        }


        // Find the other game objects in the scene
        gameManager = GameObject.FindWithTag("GameManager");

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

        hangmanCore = GameObject.FindObjectOfType<Core>().GetComponent<Core>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManager != null)
        {
            gameManager.GetComponent<GameManager>().enabled = false;
        }
        else if (gameManager == null)
        {
            Debug.LogError("GameManager object not found!");
            gameManager = GameObject.FindGameObjectWithTag("GameManager");
        }

        // Find the three game objects in the scene
        DisplayButtons = GameObject.FindGameObjectWithTag("DisplayButtons");
        GameButtons = GameObject.FindGameObjectWithTag("GameButtons");
        ThemeDiffButtons = GameObject.FindGameObjectWithTag("ThemeDiffButtons");
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        theme = GameObject.FindGameObjectWithTag("ThemeTitle").GetComponent<TMP_Text>();
        wordDisplay = GameObject.FindGameObjectWithTag("wordDisplay").GetComponent<TMP_Text>();
        letterBank = GameObject.FindGameObjectWithTag("LetterBank");
        EndStateScreen = GameObject.FindGameObjectWithTag("EndState");
    }

    private void OnEnable()
    {
        gameManager.GetComponent<GameManager>().enabled = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            // Show the DisplayButtons and hide the others
            DisplayButtons.SetActive(false);
            GameButtons.SetActive(true);
            ThemeDiffButtons.SetActive(false);

            // Sets EndStateScreen is Inactive on start
            EndStateScreen.SetActive(false);
        }
        else if (scene.name == "Config")
        {
            // Show the ThemeDiffButtons and hide the others
            DisplayButtons.SetActive(false);
            GameButtons.SetActive(false);
            ThemeDiffButtons.SetActive(true);
        }
        else
        {
            // Show the DisplayButtons and hide the others
            DisplayButtons.SetActive(true);
            GameButtons.SetActive(false);
            ThemeDiffButtons.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameManager.GetComponent<GameManager>().isInRunningGame == true)
        {
            if (SceneManager.GetActiveScene().name != "Main")
                return;

            currentSession = gameManager.GetComponent<GameManager>().sessionDifficulty;
            getWinStreak = gameManager.GetComponent<GameManager>().winStreak;
            isIronOn = DetermineIronMode(gameManager.GetComponent<GameManager>().isIronmanMode);

            // Update the word list for the chosen theme
            string[] words;
            if (chosenTheme == "All")
            {
                words = Dictionary.GetAllWords();
            }
            else
            {
                words = Dictionary.GetWordsByTheme(chosenTheme);
            }

            if (_currentWord != hangmanCore.currentWord)
            {
                _currentWord = hangmanCore.currentWord;
                UpdateWordDisplay(_currentWord, isIronOn, currentSession);
            }

            string _updated = Mathf.RoundToInt(Mathf.Abs(hangmanCore.remainingTimeToSolve)).ToString();
            timeRemainingDisplay.text = _updated;
        }
    }

    internal void UpdateWordDisplay(string currentWord, bool isIronOn, SessionDifficulty sessionDifficulty)
    {
        _currentWord = currentWord;

        string display = "";
        foreach (char c in currentWord)
        {
            if (c == ' ')
            {
                display += " ";
            }
            else if (guessedLetters.Contains(c.ToString().ToLower()))
            {
                display += c;
            }
            else if (c == '-' || c == '\'')
            {
                if (isIronOn || sessionDifficulty == SessionDifficulty.Hard)
                {
                    display += " ";
                }
                else
                {
                    display += c;
                }
            }
            else
            {
                display += "_";
            }
        }

        if (display.IndexOf("_") == -1)
        {
            Debug.Log("Word is complete!");
            WordIsComplete();
        }

        if (wordDisplay != null)
        {
            wordDisplay.text = display;
        }
        else
        {
            Debug.Log("wordDisplay is null");
        }
    }

    public bool DetermineIronMode(bool isSingle)
    {
        return isSingle;
    }

    internal void UpdateLetterBankDisplay()
    {
        if (letterBank == null)
            return;

        TextMeshProUGUI letterBankText = letterBank.GetComponent<TextMeshProUGUI>();
        letterBankText.text = "Letter Bank: ";
        foreach (string letter in guessedLetters)
        {
            letterBankText.text += letter + ", ";
        }
    }

    internal void UpdateLetterDisplay(int i, string guess)
    {
        // Convert the guessed letter to lowercase for case-insensitive comparison
        string lowerCaseGuess = guess.ToLower();

        // Check if the letter has already been guessed
        if (guessedLetters.Contains(lowerCaseGuess))
        {
            Debug.Log($"Letter {lowerCaseGuess} has already been guessed");
            return;
        }

        // Update the display with the guessed letter at position i
        guessedLetters.Add(lowerCaseGuess);
        Debug.Log($"Updated guessed letters: {string.Join(", ", guessedLetters)}");
        UpdateLetterBankDisplay();

        if (_currentWord.ToLower().Contains(lowerCaseGuess))
        {
            _matchIndices.Clear();
            for (int j = 0; j < _currentWord.Length; j++)
            {
                if (_currentWord[j].ToString().ToLower().Equals(lowerCaseGuess))
                {
                    _matchIndices.Add(j);
                }
            }

            hangmanCore.MatchIndices = _matchIndices;
            Debug.Log($"Match found: {lowerCaseGuess} at indices {string.Join(", ", _matchIndices)}");

            foreach (int index in _matchIndices)
            {
                _currentWord = _currentWord.Substring(0, index) + _currentWord[index].ToString().ToLower() + _currentWord.Substring(index + 1);
                Debug.Log($"Updated current word: {_currentWord}");
            }

            UpdateWordDisplay(_currentWord, isIronOn, currentSession);
        }
    }



    internal bool WordIsComplete()
    {
        bool isComplete = guessedLetters.Count == _currentWord.Replace(" ", "").Length;
        if (isComplete && hangmanCore.runningSession && !isWordCompleted)
        {
            hangmanCore.wordCompleted = true;
            DisplaySuccessWord();
            EndStateScreen.SetActive(true);
            isWordCompleted = true;
        }

        Debug.Log("Word is completed");
        return isComplete;
    }

    internal void UpdateGuessesDisplay(int allowedGuesses)
    {
        Debug.Log($"Guesses left: {allowedGuesses}");
    }

    internal void RevealWord()
    {
        Debug.Log("Revealing word: " + _currentWord);
    }

    internal void DisplaySuccessWord()
    {
        EndStateScreen.SetActive(true);
        GameManager gameManager = FindObjectOfType<GameManager>();
        _isEndStateDisplayed = true;

        hangmanCore.SolvedWordSuccessfully();

        WinScreen.SetActive(true);
        LoseScreen.SetActive(false);

        Debug.Log("Successfully solved word.");
    }

    internal void DisplayVictory()
    {
        if (_isEndStateDisplayed)
            return;

        if (hangmanCore.wordCompleted == true)
        {
            WinScreen.SetActive(true);
            LoseScreen.SetActive(false);
        }

        Debug.Log("Displaying victory message");
        _isEndStateDisplayed = true;
    }

    internal void DisplayDefeat()
    {
        if (_isEndStateDisplayed)
            return;

        gameManager.GetComponent<GameManager>().winStreak = 0;
        hangmanCore.failWordCompletion = true;

        WinScreen.SetActive(false);
        LoseScreen.SetActive(true);

        _isEndStateDisplayed = true;
    }

    internal void UpdateTimeDisplay(float remainingTimeToSolve)
    {
        if (timeRemainingDisplay != null)
        {
            int minutes = Mathf.FloorToInt(remainingTimeToSolve / 60f);
            int seconds = Mathf.FloorToInt(remainingTimeToSolve % 60f);
            timeRemainingDisplay.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
        else
        {
            Debug.Log("timeRemainingDisplay is null");
        }
    }
}
