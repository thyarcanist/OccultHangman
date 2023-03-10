using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageRandomizer : MonoBehaviour
{
    public Image panelBackground;
    public Sprite panelBorder;
    public Sprite[] Sprites;

    private int randomIndex;



    private void Awake()
    {
        panelBackground = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        randomIndex = Random.Range(0, Sprites.Length);
    }

    void RandomizeBackground()
    {
        panelBackground.sprite = Sprites[randomIndex];
    }
}
