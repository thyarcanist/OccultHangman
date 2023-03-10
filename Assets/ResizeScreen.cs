using UnityEngine;

public class ResizeScreen : MonoBehaviour
{
    // Set the resolution of the game screen
    public float screenWidth = 1080f;
    public float screenHeight = 1920f;

    void Start()
    {
        // Get the screen dimensions of the phone
        float phoneWidth = Screen.width;
        float phoneHeight = Screen.height;

        // Calculate the aspect ratio of the phone
        float phoneAspectRatio = phoneWidth / phoneHeight;

        // Calculate the aspect ratio of the game screen
        float screenAspectRatio = screenWidth / screenHeight;

        // Check if the phone's aspect ratio is wider than the game screen's aspect ratio
        if (phoneAspectRatio > screenAspectRatio)
        {
            // Calculate the new width of the game screen
            float newScreenWidth = screenHeight * phoneAspectRatio;

            // Set the new resolution of the game screen
            Screen.SetResolution((int)newScreenWidth, (int)screenHeight, true);
        }
        else // Phone's aspect ratio is narrower than the game screen's aspect ratio
        {
            // Calculate the new height of the game screen
            float newScreenHeight = screenWidth / phoneAspectRatio;

            // Set the new resolution of the game screen
            Screen.SetResolution((int)screenWidth, (int)newScreenHeight, true);
        }
    }
}