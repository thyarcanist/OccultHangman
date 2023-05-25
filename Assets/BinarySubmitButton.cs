using UnityEngine;
using UnityEngine.UI;

public class BinarySubmitButton : MonoBehaviour
{
    private Core hangmanCore;
    private BinaryDictionary binaryDictionary;

    private void Awake()
    {
        hangmanCore = FindObjectOfType<Core>().GetComponent<Core>();
        binaryDictionary = FindObjectOfType<BinaryDictionary>().GetComponent<BinaryDictionary>();

        // Set the onClick listener for the button
        GetComponent<Button>().onClick.AddListener(Submit);
    }

    public void Submit()
    {
        string guess = binaryDictionary.currentBinaryInput;
        hangmanCore.ProcessGuess(guess);
        binaryDictionary.currentBinaryInput = "";
    }
}