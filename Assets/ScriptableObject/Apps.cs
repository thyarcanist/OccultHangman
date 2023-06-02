using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Apps", menuName = "Hangman/App")]
public class Apps : ScriptableObject
{
    public Sprite appImage;
    public string appName;
    public string appMotto;
    public bool isUnlocked = false;
}
