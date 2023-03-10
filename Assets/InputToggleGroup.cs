using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputToggleGroup : MonoBehaviour
{
    public Toggle binaryToggle;
    public Toggle digitToggle;
    public Toggle textToggle;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        // Set the binary toggle to be on by default
        binaryToggle.isOn = true;
        ShowBinary();
    }

    public void ShowBinary()
    {
        gameManager.isUsingInput = false;
        gameManager.isUsingNumpad = false;
        gameManager.isUsingBinary = true;
    }

    public void ShowDigit()
    {
        gameManager.isUsingInput = false;
        gameManager.isUsingNumpad = true;
        gameManager.isUsingBinary = false;
    }

    public void ShowText()
    {
        gameManager.isUsingInput = true;
        gameManager.isUsingNumpad = false;
        gameManager.isUsingBinary = false;
    }
}