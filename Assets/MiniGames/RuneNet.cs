using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuneNet : MonoBehaviour
{
    private string AppName = "RUNE.NET";
    private string AppMotto = "Create Runes,";
    public bool isUnlocked = false;

    private void Awake()
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = AppName;
    }
}
