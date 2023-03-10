using UnityEngine;
using UnityEngine.UI;

public class BinarySubmitButton : MonoBehaviour
{
    public GameManager gameManager;
    public BinaryDictionary binaryDictionary;

    private void Awake()
    {
        // Set the onClick listener for the button
        GetComponent<Button>().onClick.AddListener(Submit);
    }

    private void Submit()
    {
        string guess = binaryDictionary.currentBinaryInput;
        gameManager.ProcessGuess(guess);
        binaryDictionary.currentBinaryInput = "";
    }
}