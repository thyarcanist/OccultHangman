using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LaunchExternalExe : MonoBehaviour
{
    private const string SecondaryGamePath = "C:/Program Files (x86)/SecondaryGame/SecondaryGame.exe"; // Replace with the correct path to SecondaryGame.exe
    private void Start()
    {
        // Check if SecondaryGame.exe exists
        if (System.IO.File.Exists(SecondaryGamePath))
        {
            // Launch SecondaryGame.exe
            Process.Start(SecondaryGamePath);
        }
        else
        {
            Debug.LogError("SecondaryGame.exe not found");
        }
    }
}
