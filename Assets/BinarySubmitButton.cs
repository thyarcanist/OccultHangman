using UnityEngine;
using UnityEngine.UI;

public class BinarySubmitButton : MonoBehaviour
{
    private GameManager gameManager;
    private Core hangmanCore;
    private BinaryDictionary binaryDictionary;

    private void Awake()
    {
        hangmanCore = FindObjectOfType<Core>().GetComponent<Core>();

        // Set the onClick listener for the button
        GetComponent<Button>().onClick.AddListener(Submit);
    }

    private void Submit()
    {
        string guess = binaryDictionary.currentBinaryInput;
        hangmanCore.ProcessGuess(guess);
        binaryDictionary.currentBinaryInput = "";
    }
}