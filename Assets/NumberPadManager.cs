using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberPadManager : MonoBehaviour
{
    public Dictionary<string, int> keypadMapping = new Dictionary<string, int>()
    {
        {"1", 0}, {"2", 1}, {"3", 2},
        {"4", 3}, {"5", 4}, {"6", 5},
        {"7", 6}, {"8", 7}, {"9", 8},
        {"*", -1}, {"0", 9}, {"#", -2}
    };

    public Dictionary<string, string> letterMapping = new Dictionary<string, string>()
    {
        {"2", "A"}, {"22", "B"}, {"222", "C"},
        {"3", "D"}, {"33", "E"}, {"333", "F"},
        {"4", "G"}, {"44", "H"}, {"444", "I"},
        {"5", "J"}, {"55", "K"}, {"555", "L"},
        {"6", "M"}, {"66", "N"}, {"666", "O"},
        {"7", "P"}, {"77", "Q"}, {"777", "R"}, {"7777", "S"},
        {"8", "T"}, {"88", "U"}, {"888", "V"},
        {"9", "W"}, {"99", "X"}, {"999", "Y"}, {"9999", "Z"}
    };

    public string currentInput = "";
    public GameManager gameManager;
    public GameObject[] numberBlocks;
    private Core hangmanCore;

    public void OnEnable()
    {
        hangmanCore = GameObject.FindObjectOfType<Core>().GetComponent<Core>();
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager could not be found.");
        }

        // Find all the number block game objects in the scene
        numberBlocks = GameObject.FindGameObjectsWithTag("NumBlock");
    }

    string letter = "";
    int maximumInput = 4;
    private string FetchLetter()
    {
        // Check if the current input exceeds the maximum possible input length
        if (currentInput.Length >= maximumInput)
        {
            currentInput = "";
        }

        // Check if the current input matches a key in the letterMapping dictionary
        if (letterMapping.ContainsKey(currentInput))
         {
                // Get the matched letter from the letterMapping dictionary
                letter = letterMapping[currentInput];

                // Clear the current input
                currentInput = "";
         }


        return letter;
    }

    public void OnNumberBlockPressed(NumberBlockController numBlock)
    {
        int numBlockIndex = (int)numBlock.numBlockType;
        string key = "";

        // Check if the pressed number block corresponds to a key in the keypadMapping dictionary
        foreach (KeyValuePair<string, int> entry in keypadMapping)
        {
            if (entry.Value == numBlockIndex)
            {
                key = entry.Key;
                break;
            }
        }

        if (key == "#")
        {
            Debug.Log("Hash is pressed");
            // Submit the guess
            if (!string.IsNullOrEmpty(currentInput))
            {
                hangmanCore.ProcessGuess(FetchLetter());
            }
            // Clear the current input
            currentInput = "";
        }
        else if (key == "*")
        {
            Debug.Log("Star is pressed");
            // Clear the current input
            currentInput = "";
        }
        else
        {
            // Add the key to the current input
            if (currentInput == null)
            {
                currentInput = "";
            }
            currentInput += key;
        }

        Debug.Log("Current Input: " + currentInput);
    }


    public void HandleSubmitButtonClick(int keyCode)
    {
        // Check if the keyCode is associated with "#"
        if (keyCode == -2 && !string.IsNullOrEmpty(currentInput))
        {
            Debug.Log("Hash is pressed");
            // Submit the guess
            if (!string.IsNullOrEmpty(currentInput))
            {
                hangmanCore.ProcessGuess(FetchLetter().ToLower());
            }
            // Clear the current input
            currentInput = "";
        }
        else if (keyCode == -1 && !string.IsNullOrEmpty(currentInput))
        {
            Debug.Log("Star is pressed");
            // Clear the current input
            currentInput = "";
        }
        else { }
    }
}
