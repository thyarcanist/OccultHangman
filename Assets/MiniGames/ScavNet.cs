using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScavNet : MonoBehaviour
{
    private string AppName = "SCAV.NET";
    private string AppMotto = "We Find, You Seek.";
    public bool isUnlocked = false;

    private void Awake()
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = AppName;
    }
}
