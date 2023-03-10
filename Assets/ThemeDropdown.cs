using System;
using TMPro;
using UnityEngine;

public class ThemeDropdown : MonoBehaviour
{
    private GameManager gameManager;
    public TMP_Dropdown dropdown;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        dropdown = GetComponent<TMP_Dropdown>();
    }

    public void Start()
    {
        dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(int value)
    {
        string selectedOption = dropdown.options[value].text;
        DictTheme selectedTheme = (DictTheme)Enum.Parse(typeof(DictTheme), selectedOption);
        GameManager.Instance.SetSelectedTheme(selectedTheme);
    }
}