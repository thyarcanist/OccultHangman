using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Riddle", menuName = "Hangman/Riddles")]
public class Riddles : ScriptableObject
{
    public string containedRiddle = "";
    public string answerToRiddle = "";
    public bool isRiddleSolved = false;
    public int riddleID;
}
