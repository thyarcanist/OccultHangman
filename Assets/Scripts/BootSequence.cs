using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BootSequence : MonoBehaviour
{
    public TMP_Text bootUpText;
    public string operatingSystemName;
    public string lastBootTime;
    public bool isProfileFound;
    public string userStatus;
    public string userName;

    public TMP_Text connectText;

    public bool connectionIsAuthorized = false;
    public bool isSequenceFinished = false;

    public string line1 = "> Alerting Processes...";
    public string line2 = $"> Booting: VXXX.SO";
    public string line3 = "ALERT! CORRUPTION FOUND";
    public string line4 = "Updating... \nUpdate Complete: VoidOS";
    public string line5 = "Initiating sub-routine: Transdimensional Signal...";
    public string line6;
    public string line7 = "Running protocol: Unknown entity detection...";
    public string line8 = "T.O.B.I alert status: INACTIVE";

    public GameObject MainMenuScreen;
    public GameObject MainGameScreen; 
    public GameObject ConfigMenuScreen;
    public GameObject EndStateScreen;

    [SerializeField]
    private string[] bootLines;
    [SerializeField]
    private GameObject afterBootSequence;

    private bool isSequenceRunning = false;

    private void Awake()
    {
        connectText = GameObject.FindGameObjectWithTag("connectButton").GetComponent<TMP_Text>();
        MainMenuScreen = GameObject.FindGameObjectWithTag("MMProps");
        MainGameScreen = GameObject.FindGameObjectWithTag("MainGame");
        ConfigMenuScreen = GameObject.FindGameObjectWithTag("ThemeCanvas");
        EndStateScreen = GameObject.FindGameObjectWithTag("EndState");

        userName = GameObject.FindObjectOfType<GameManager>().agentProxyID.ToString();
        if (!isProfileFound)
        {
            userStatus = "UNKNOWN";
            connectText.text = "AUTHENTICATE USER";
        }
        else { userStatus = "FOUND"; connectText.text = "CONNECT"; }

        line6 = $"User profile status: {userStatus}";

        bootLines = new string[8];
        bootUpText = gameObject.GetComponentInChildren<TMP_Text>();
        if (bootUpText != null)
        {
            connectionIsAuthorized = true;
        }
        afterBootSequence = GameObject.FindGameObjectWithTag("AfterBSQ");
        afterBootSequence.SetActive(false);
}

    private void OnEnable()
    {
        if (isProfileFound)
        {
            lastBootTime = System.DateTime.Now.ToString("yyyy.MM.dd");
            bootLines[1] = "> Booting: VoidOS";
            bootLines[2] = "";
            bootLines[3] = "";
        }
        else
        {
            lastBootTime = "2023.03.14";
            bootLines[1] = line2;
            bootLines[2] = line3;
            bootLines[3] = line4;
        }
        bootLines[0] = line1;
        bootLines[4] = line5;
        bootLines[5] = line6;
        bootLines[6] = line7;
        bootLines[7] = line8;
    }

    private void Start()
    {

        MainMenuScreen.SetActive(false);
        MainGameScreen.SetActive(false);
        ConfigMenuScreen.SetActive(false);
        EndStateScreen.SetActive(false);
    }

    private void Update()
    {
        if (connectionIsAuthorized && !isSequenceRunning && !isSequenceFinished)
        {
            StartCoroutine(RunSequence());
        }
        if (isSequenceFinished) { afterBootSequence.SetActive(true); }
    }

    private IEnumerator RunSequence()
    {
        isSequenceRunning = true;

        for (int i = 0; i < bootLines.Length; i++)
        {
            foreach (char c in bootLines[i])
            {
                bootUpText.text += c;
                yield return new WaitForSeconds(0.05f);
            }
            bootUpText.text += "\n";

            // Adding a pause after line 5 and 7
            if (i == 4 || i == 6)
            {
                yield return new WaitForSeconds(2.0f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        isSequenceFinished = true;
        isSequenceRunning = false;
    }
}
