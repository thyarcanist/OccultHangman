using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.PlayerLoop.PreUpdate;

public class Core : MonoBehaviour
{
    // Hangman Logic --> Will be placed here.

    [Header("Word")]
    [SerializeField] string currentString;
    [SerializeField] string getScene;
    [SerializeField] private string currentWord;
    [SerializeField] string _currentWord;
    [SerializeField] string currentGuess = "";

    private List<int> _matchIndices = new List<int>();
    public List<int> MatchIndices
    {
        get => _matchIndices;
        set => _matchIndices = value;
    }

    public int currentSessionLives;


    public int IRONMAN_MODE_EASY_GUESSES { get; private set; } = 20;
    public int IRONMAN_MODE_MEDIUM_GUESSES { get; private set; } = 16;
    public int IRONMAN_MODE_HARD_GUESSES { get; private set; } = 6;

    [Header("Logic: Lists")]
    [SerializeField] public List<string> solvedWords = new List<string>();
    [SerializeField] private List<string> usedWords = new List<string>();

    [SerializeField] private bool isTimeRemainingCodeRunning = false;


    [Header("Standard Game Logic Variables")]
    public bool runningSession;
    public int wordsPerExtraLife = 3;
    public bool isProcessingGuess = false;
    public bool failWordCompletion = false;

    public int allowedGuesses = 6;
    public int initialGuessAmount;

    public bool wordCompleted = false;

    [SerializeField] private float maxSolveTime = 3f; // in Minutes
    public float remainingTimeToSolve;

    [SerializeField] private int numberOfWordsSolved;
    [SerializeField] private DisplayManager _displayManager;
    [SerializeField] private GameManager gameManager;


    #region WordSelection
    private string GetRandomWord()
    {
        string[] possibleWords;

        if (gameManager.selectedTheme == DictTheme.All)
        {
            possibleWords = gameManager._dictionary.GetAllWords();
        }
        else
        {
            possibleWords = gameManager._dictionary.GetWordsByTheme(gameManager.selectedTheme.ToString());
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

            if (gameManager.isIronmanMode)
            {
                if (gameManager.sessionDifficulty == SessionDifficulty.Easy)
                {
                    initialGuessAmount = IRONMAN_MODE_EASY_GUESSES;
                }
                else if (gameManager.sessionDifficulty == SessionDifficulty.Normal)
                {
                    initialGuessAmount = IRONMAN_MODE_MEDIUM_GUESSES;
                }
                else if (gameManager.sessionDifficulty == SessionDifficulty.Hard)
                {
                    initialGuessAmount = IRONMAN_MODE_HARD_GUESSES;
                }
            }

            _displayManager.UpdateWordDisplay(currentWord, gameManager.isIronmanMode, gameManager.sessionDifficulty);
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

                    if (numberOfWordsSolved % wordsPerExtraLife == 0 && gameManager.isIronmanMode)
                    {
                        gameManager.maxSessionLives++;
                    }

                    if (gameManager.maxSessionLives > 5)
                    {
                        gameManager.maxSessionLives = 5;
                    }

                    if (numberOfWordsSolved == 5 && gameManager.isIronmanMode)
                    {
                        allowedGuesses++;
                    }

                    if (solvedWords.Count == gameManager._dictionary.GetAllWords().Length)
                    {
                        EndSession(true);
                    }
                    else
                    {
                        GetNewWord();
                        _displayManager.UpdateWordDisplay(currentWord, gameManager.isIronmanMode, gameManager.sessionDifficulty);
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
                    if (gameManager.isIronmanMode)
                    {
                        wordsPerExtraLife--;
                        if (wordsPerExtraLife == 0)
                        {
                            wordsPerExtraLife = 3;
                            gameManager.maxSessionLives++;
                        }
                    }

                    if (currentSessionLives < gameManager.minSessionLives)
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
        if (gameManager.isUsingBinary)
        {
            if (gameManager.binaryDictionary.currentBinaryInput.Length < BinaryDictionary.maxBinaryDigitCount)
            {
                gameManager.binaryDictionary.currentBinaryInput += guess.ToLower();
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

            if (gameManager.isUsingNumpad)
            {
                gameManager.numpadController.currentInput += guessString;
            }
            else if (gameManager.isUsingInput)
            {
                if (gameManager.userInput.text.Length < 1)
                {
                    gameManager.userInput.text += guessString;
                }
            }
        }
    }
    public void SubmitGuess()
    {
        string guess = gameManager.userInput.text;
        if (!string.IsNullOrEmpty(guess))
        {
            ProcessGuess(guess);
            gameManager.ClearInput();
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
            gameManager.winStreak++;
        }
    }


    #region EndGame&DisplayLogic
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

    #endregion

    #region RUNTIME
    private void Awake()
    {

        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager = gameManager.GetComponent<GameManager>();

        _displayManager = FindObjectOfType<DisplayManager>();
        _displayManager = GameObject.FindWithTag("DisplayManager").GetComponent<DisplayManager>();

        if (gameManager != null && gameManager.DisplayManager != null)
        {
            gameManager.DisplayManager.Dictionary = gameManager._dictionary;
        }
        else
        {
            Debug.LogWarning("GameManager or DisplayManager not found.");
        }

    }
    private void Update()
    {
        _displayManager.UpdateWordDisplay(_currentWord, gameManager.isIronmanMode, gameManager.sessionDifficulty);

        if (gameManager.canUpdate)
        {
            if (runningSession)
            {
                isTimeRemainingCodeRunning = true; // set to true when the code block runs

                if (gameManager.isUsingInput)
                {
                    gameManager.inputManager.SetInputActive();
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SubmitGuess();
                    }
                }
                else if (gameManager.isUsingNumpad)
                {
                    gameManager.inputManager.SetNumpadActive();
                    string guess = gameManager.numpadController.currentInput;
                }
                else if (gameManager.isUsingBinary)
                {
                    gameManager.inputManager.SetBinaryActive();
                    string guess = gameManager.binaryDictionary.currentBinaryInput;
                    int? matchedKey = gameManager.binaryDictionary.GetMatchedKey(guess);
                    if (matchedKey.HasValue)
                    {
                        char matchChar = (char)matchedKey.Value;
                        ProcessGuess(matchChar.ToString());
                        gameManager.binaryDictionary.currentBinaryInput = "";
                    }
                }


                if (allowedGuesses == 0)
                {
                    if (gameManager.isIronmanMode)
                    {
                        wordsPerExtraLife--;
                        if (wordsPerExtraLife == 0)
                        {
                            wordsPerExtraLife = 3;
                            gameManager.maxSessionLives++;
                        }
                    }
                    else
                    {
                        currentSessionLives--;
                    }

                    if (currentSessionLives < gameManager.minSessionLives)
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

    #endregion

}

