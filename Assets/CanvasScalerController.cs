using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerController : MonoBehaviour
{
    // Set this to the resolution that the UI was designed for (e.g. 1080p)
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    void Start()
    {
        // Get the CanvasScaler component attached to the Canvas game object
        CanvasScaler scaler = GetComponent<CanvasScaler>();

        // Set the reference resolution to match the designed resolution
        scaler.referenceResolution = referenceResolution;

        // Calculate the screen aspect ratio
        float screenAspectRatio = Screen.width / (float)Screen.height;

        // Calculate the reference aspect ratio
        float referenceAspectRatio = referenceResolution.x / referenceResolution.y;

        // Calculate the scale factor
        float scaleFactor = 1.0f;
        if (screenAspectRatio < referenceAspectRatio)
        {
            // Screen is taller than the reference resolution, scale based on height
            scaleFactor = Screen.height / referenceResolution.y;
        }
        else
        {
            // Screen is wider than the reference resolution, scale based on width
            scaleFactor = Screen.width / referenceResolution.x;
        }

        // Set the scale factor on the CanvasScaler component
        scaler.scaleFactor = scaleFactor;

        // Ensure that the scale factor is an integer multiple of the reference scale factor
        scaler.scaleFactor = Mathf.FloorToInt(scaler.scaleFactor / scaler.referencePixelsPerUnit) * scaler.referencePixelsPerUnit;

        // Set the reference pixels per unit to ensure that the UI is pixel perfect
        scaler.referencePixelsPerUnit = 1.0f;
    }
}
