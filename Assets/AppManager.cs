using UnityEngine;

public class AppManager : MonoBehaviour
{
    // Base Menu Screen -- Story Mode
    // This script is not implemented yet

    [SerializeField]
    public Apps[] Apps;
    [SerializeField]
    public GameObject[] apps;
    public enum AppType 
    { 
        None,
        Scavnet,
        ApolloSight,
        TeamName,
        SpecReq,
        CodexInfo,
        HelBatch,
        Hangman
    }

    [SerializeField]
    public AppType selectedApp;


    // App Tags
    private static string scavnet = "SNET";
    private static string apollosight = "APS";
    private static string teamname = "TMNM";
    private static string specreqs = "SRQ";
    private static string codexinfo = "CDIFMA";
    private static string hellbatch = "HELBATCH";
    // private static string hangman = "OCCHANG";




    private void Awake()
    {
        apps = new GameObject [6]; 
    }

    private void OnEnable()
    {
        selectedApp = AppType.None;

        GetAppReferences();
        switch (selectedApp)
        {
            case AppType.Scavnet:
                break;
        }
    }

    public void GetSelectedChosenApp()
    {
        if (selectedApp == AppType.Scavnet) { return; }
        else if (selectedApp == AppType.ApolloSight) { return; }
        else if (selectedApp == AppType.TeamName) { return; }
        else if (selectedApp == AppType.SpecReq) { return; }
        else if (selectedApp == AppType.CodexInfo) { return; }
        else if (selectedApp == AppType.HelBatch) { return; }
        else if (selectedApp == AppType.Hangman) { return; }
        else
        {
            selectedApp = AppType.None;
            Debug.Log("No App Chosen");
            // Base Menu Screen -- Story Mode
        }
    }

    private void GetAppReferences()
    {
        apps[0] = GameObject.FindGameObjectWithTag(scavnet);
        apps[1] = GameObject.FindGameObjectWithTag(apollosight);
        apps[2] = GameObject.FindGameObjectWithTag(teamname);
        apps[3] = GameObject.FindGameObjectWithTag(specreqs);
        apps[4] = GameObject.FindGameObjectWithTag(codexinfo);
        apps[5] = GameObject.FindGameObjectWithTag(hellbatch);
    }
}
