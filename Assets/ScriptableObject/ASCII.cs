using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ASCII", menuName = "Hangman/ASCII")]
public class ASCII : ScriptableObject
{
    public RenderTexture asciiImage;
    public string whatsThatASCII = "";
    public bool isASCIISolved = false;
    public int asciiID;
}
