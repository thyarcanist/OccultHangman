using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputSubmitButton : MonoBehaviour
{
    public TMP_InputField inputField;

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
            GameManager.Instance.ProcessGuess(guess);
            inputField.text = "";
        }
    }
}