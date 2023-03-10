using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayManager : MonoBehaviour
{
    public GameObject gameManager;
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
    public string chosenTheme;
    private string _currentWord;

    public TMP_Text theme;
    public GameObject DisplayButtons;
    public GameObject ThemeDiffButtons;
    public GameObject GameButtons;

    private bool _isEndStateDisplayed = false;
    public GameObject EndStateScreen;
    public GameObject WinScreen;
    public GameObject LoseScreen;

    private List<int> _matchIndices = new List<int>();
    public List<string> guessedLetters = new List<string>();
    public GameObject letterBank;

    public TMP_Text wordDisplay = null;
    public TMP_Text timeRemainingDisplay = null;
    bool isComplete;

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

        gameManager.GetComponent<GameManager>().enabled = false;

    }
    private void OnEnable()
    {
        gameManager.GetComponent<GameManager>().enabled = true;

        // Find the three game objects in the scene
        DisplayButtons = GameObject.FindGameObjectWithTag("DisplayButtons");
        GameButtons = GameObject.FindGameObjectWithTag("GameButtons");
        ThemeDiffButtons = GameObject.FindGameObjectWithTag("ThemeDiffButtons");
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        theme = GameObject.FindGameObjectWithTag("ThemeTitle").GetComponent<TMP_Text>();
        wordDisplay = GameObject.FindGameObjectWithTag("wordDisplay").GetComponent<TMP_Text>();
        letterBank = GameObject.FindGameObjectWithTag("LetterBank");
        EndStateScreen = GameObject.FindGameObjectWithTag("EndState");

        // Show the DisplayButtons and hide the others
        DisplayButtons.SetActive(true);
        GameButtons.SetActive(false);
        ThemeDiffButtons.SetActive(false);

        // Sets EndStateScreen is Inactive on start
        EndStateScreen.SetActive(false);
    }

    private string _previousWord = "";
    public string sceneName;

    private void Update()
    {
        chosenTheme = gameManager.GetComponent<GameManager>().selectedTheme.ToString();
        theme.text = $"Theme: " + chosenTheme;
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

        // Check the current scene and show/hide the appropriate game object
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Menu")
        {
            gameManager.GetComponent<GameManager>().isInMainMenu = true;
            DisplayButtons.SetActive(true);
            GameButtons.SetActive(false);
            ThemeDiffButtons.SetActive(false);
        }
        else if (sceneName == "Config")
        {
            gameManager.GetComponent<GameManager>().isInConfig = true;
            DisplayButtons.SetActive(false);
            GameButtons.SetActive(false);
            ThemeDiffButtons.SetActive(true);
        }
        else if (sceneName == "Main")
        {
            gameManager.GetComponent<GameManager>().isInRunningGame = true;
            DisplayButtons.SetActive(false);
            GameButtons.SetActive(true);
            ThemeDiffButtons.SetActive(false);

            if (_currentWord != _previousWord)
            {
                UpdateWordDisplay(_currentWord, isIronOn, currentSession);
                _previousWord = _currentWord;
            }
        }
        UpdateWordDisplay(_currentWord, isIronOn, currentSession);

        string _updated = Mathf.RoundToInt(Mathf.Abs(gameManager.GetComponent<GameManager>().remainingTimeToSolve)).ToString();
        timeRemainingDisplay.text = _updated;
    }

    internal void UpdateWordDisplay(string currentWord, bool isIronOn, SessionDifficulty sessionDifficulty)
    {
        Debug.Log($"Updating word display with current word: {currentWord}");

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

        Debug.Log($"Updated word display: {display}");

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
        if (isSingle)
        {
            return isIronOn = true;
        }
        else { return isIronOn = false; }
    }
    private void UpdateLetterBankDisplay()
    {
        if (letterBank == null) return;

        TextMeshProUGUI letterBankText = letterBank.GetComponent<TextMeshProUGUI>();
        letterBankText.text = "Letter Bank: ";
        foreach (string letter in guessedLetters)
        {
            letterBankText.text += letter + ", ";
        }
    }
    internal void UpdateLetterDisplay(int i, string guess)
    {
        // Check if the letter has already been guessed
        if (guessedLetters.Contains(guess))
        {
            Debug.Log($"Letter {guess} has already been guessed"); // For testing
            return;
        }

        // Update the display with the guessed letter at position i
        guessedLetters.Add(guess);
        Debug.Log($"Updated guessed letters: {string.Join(", ", guessedLetters)}"); // For testing
        UpdateLetterBankDisplay();

        if (_currentWord.Contains(guess))
        {
            _matchIndices.Clear(); // Clear the previous indices
            for (int j = 0; j < _currentWord.Length; j++)
            {
                if (_currentWord[j].ToString().Equals(guess))
                {
                    _matchIndices.Add(j);
                }
            }

            GameManager.Instance.MatchIndices = _matchIndices; // Set the matched indices in GameManager
            Debug.Log($"Match found: {guess} at indices {string.Join(", ", _matchIndices)}"); // For testing

            // Update the current word with the matched letter at the matched indices
            foreach (int index in _matchIndices)
            {
                _currentWord = _currentWord.Substring(0, index) + guess + _currentWord.Substring(index + 1);
                Debug.Log($"Updated current word: {_currentWord}"); // For testing
            }

            UpdateWordDisplay(_currentWord, isIronOn, currentSession); // Add this line to update the word display
        }
    }




    internal bool WordIsComplete()
    {
        bool isComplete = guessedLetters.Count == _currentWord.Replace(" ", "").Length;
        if (isComplete && gameManager.GetComponent<GameManager>().runningSession)
        {
            gameManager.GetComponent<GameManager>().wordCompleted = true;
            DisplaySuccessWord();
            EndStateScreen.SetActive(true);
        }
        return isComplete;
    }



    internal void UpdateGuessesDisplay(int allowedGuesses)
    {
        // Update the display with the number of guesses left
        Debug.Log($"Guesses left: {allowedGuesses}"); // For testing
    }
    internal void RevealWord()
    {
        // Reveal the word after the game is over
        Debug.Log("Revealing word: " + _currentWord); // For testing
    }

    internal void DisplaySuccessWord()
    {
        EndStateScreen.SetActive(true);
        GameManager gameManager = FindObjectOfType<GameManager>();
        _isEndStateDisplayed = true;

        gameManager.SolvedWordSuccessfully();

        WinScreen.SetActive(true);
        LoseScreen.SetActive(false);
   

        // Display word success
        Debug.Log("Successfully solved word.");
    }


    internal void DisplayVictory()
    {
        // Check if the end state screen is already displayed
        if (_isEndStateDisplayed) return;

        if (gameManager.GetComponent<GameManager>().wordCompleted == true)
        {
            WinScreen.SetActive(true);
            LoseScreen.SetActive(false);
        }

        // Display a victory message
        Debug.Log("Displaying victory message"); // For testing

        _isEndStateDisplayed = true;
    }

    internal void DisplayDefeat()
    {
        // Check if the end state screen is already displayed
        if (_isEndStateDisplayed) return;

        gameManager.GetComponent<GameManager>().winStreak = 0;
        gameManager.GetComponent<GameManager>().failWordCompletion = true;
        // Add method in GameManager
        WinScreen.SetActive(false);
        LoseScreen.SetActive(true);

        _isEndStateDisplayed = true;
    }

    internal void UpdateTimeDisplay(float remainingTimeToSolve)
    {
        if (timeRemainingDisplay != null) // Check if timeRemainingDisplay is not null
        {
            int minutes = Mathf.FloorToInt(remainingTimeToSolve / 60f);
            int seconds = Mathf.FloorToInt(remainingTimeToSolve % 60f);
            timeRemainingDisplay.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
        else
        {
            Debug.Log("timeRemainingDisplay is null"); // For testing
        }
    }

}
