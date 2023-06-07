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
        Debug.Log("Pressed Submit");
        binaryDictionary.didSubmit = true;
        string guess = binaryDictionary.currentBinaryInput;
        char? matchedKey = binaryDictionary.GetMatchedKey(guess);

        if (matchedKey.HasValue)
        {
            Debug.Log("Found value: " + matchedKey.Value);
            hangmanCore.ProcessGuess(matchedKey.ToString());
        }

        binaryDictionary.currentBinaryInput = "";
    }

}