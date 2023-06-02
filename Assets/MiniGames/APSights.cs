using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class APSights : MonoBehaviour
{
    private string AppName = "Apollo Sights";
    private string AppMotto = "What about it?";
    public bool isUnlocked = false;

    private void Awake()
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = AppName;
    }
}
