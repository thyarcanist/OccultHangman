using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class CanvasAutoScaler : MonoBehaviour
{
    [SerializeField]
    private float referenceWidth = 1080.0f;

    [SerializeField]
    private float referenceHeight = 1920.0f;

    private CanvasScaler canvasScaler;

    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }

    private void Start()
    {
        UpdateCanvasScale();
    }

    private void Update()
    {
        if (Screen.width != canvasScaler.referenceResolution.x || Screen.height != canvasScaler.referenceResolution.y)
        {
            UpdateCanvasScale();
        }
    }

    private void UpdateCanvasScale()
    {
        float widthRatio = Screen.width / referenceWidth;
        float heightRatio = Screen.height / referenceHeight;
        float scaleRatio = Mathf.Min(widthRatio, heightRatio);

        canvasScaler.referenceResolution = new Vector2(Screen.width / scaleRatio, Screen.height / scaleRatio);
    }
}