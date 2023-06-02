using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelBatch : MonoBehaviour
{
    private string AppName = "Hel.Batch";
    private string AppMotto = "cd secrets,";
    public bool isUnlocked = false;

    private void Awake()
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = AppName;
    }
}
