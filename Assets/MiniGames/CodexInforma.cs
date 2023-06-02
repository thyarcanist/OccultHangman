using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodexInforma : MonoBehaviour
{
    private string AppName = "Codex-Informa";
    private string AppMotto = "Get Gnosis";
    public bool isUnlocked = false;

    private void Awake()
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = AppName;
    }

    private void Update()
    {
        if (!isUnlocked)
        {
            StartCoroutine(CheckForAppStatus());
        }
    }


    private bool TEST = true;
    IEnumerator CheckForAppStatus()
    {
        yield return new WaitForSeconds(30);
        if (!isUnlocked && TEST)
        {
            isUnlocked = true;
        }
    }
}
