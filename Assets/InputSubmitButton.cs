using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputSubmitButton : MonoBehaviour
{
    public TMP_InputField inputField;
    private Core hangmanCore;


    private void Awake()
    {
        hangmanCore = GameObject.FindObjectOfType<Core>().GetComponent<Core>();
    }

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(SubmitInput);
    }

    private void SubmitInput()
    {
        string guess = inputField.text;
        if (!string.IsNullOrEmpty(guess))
        {
            hangmanCore.ProcessGuess(guess);
            inputField.text = "";
        }
    }
}