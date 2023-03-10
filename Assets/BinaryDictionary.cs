using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryDictionary : MonoBehaviour
{
    public static readonly Dictionary<char, string> lowercaseBinaryDict = new Dictionary<char, string>()
    {
        { 'a', "01100001" },
        { 'b', "01100010" },
        { 'c', "01100011" },
        { 'd', "01100100" },
        { 'e', "01100101" },
        { 'f', "01100110" },
        { 'g', "01100111" },
        { 'h', "01101000" },
        { 'i', "01101001" },
        { 'j', "01101010" },
        { 'k', "01101011" },
        { 'l', "01101100" },
        { 'm', "01101101" },
        { 'n', "01101110" },
        { 'o', "01101111" },
        { 'p', "01110000" },
        { 'q', "01110001" },
        { 'r', "01110010" },
        { 's', "01110011" },
        { 't', "01110100" },
        { 'u', "01110101" },
        { 'v', "01110110" },
        { 'w', "01110111" },
        { 'x', "01111000" },
        { 'y', "01111001" },
        { 'z', "01111010" }
    };
    public static readonly Dictionary<char, string> uppercaseBinaryDict = new Dictionary<char, string>()
    {
        { 'A', "01000001" },
        { 'B', "01000010" },
        { 'C', "01000011" },
        { 'D', "01000100" },
        { 'E', "01000101" },
        { 'F', "01000110" },
        { 'G', "01000111" },
        { 'H', "01001000" },
        { 'I', "01001001" },
        { 'J', "01001010" },
        { 'K', "01001011" },
        { 'L', "01001100" },
        { 'M', "01001101" },
        { 'N', "01001110" },
        { 'O', "01001111" },
        { 'P', "01010000" },
        { 'Q', "01010001" },
        { 'R', "01010010" },
        { 'S', "01010011" },
        { 'T', "01010100" },
        { 'U', "01010101" },
        { 'V', "01010110" },
        { 'W', "01010111" },
        { 'X', "01011000" },
        { 'Y', "01011001" },
        { 'Z', "01011010" }
    };

    public string currentBinaryInput = "";

    public GameObject ImageA; // 0
    public GameObject ImageB; // 1

    public static int maxBinaryDigitCount = 8;

    public void AddBinaryDigit(int binaryDigit)
    {
        currentBinaryInput += binaryDigit.ToString();
    }

    private void OnEnable()
    {
        currentBinaryInput = "";
    }

    private void OnDisable()
    {
        currentBinaryInput = "";
    }

    public float timer = 0.0f;

    public void BinaryT0()
    {
        currentBinaryInput += "0";
    }

    public void BinaryT1()
    {
        currentBinaryInput += "1";
    }

    private void Update()
    {

        if (currentBinaryInput.Length == maxBinaryDigitCount)
        {
            // Check for a match in lowercase and uppercase dictionaries
            foreach (KeyValuePair<char, string> kvp in lowercaseBinaryDict)
            {
                if (kvp.Value == currentBinaryInput)
                {
                    Debug.Log("Match found: " + kvp.Key);
                    currentBinaryInput = "";
                    return;
                }
            }
            foreach (KeyValuePair<char, string> kvp in uppercaseBinaryDict)
            {
                if (kvp.Value == currentBinaryInput)
                {
                    Debug.Log("Match found: " + kvp.Key);
                    currentBinaryInput = "";
                    return;
                }
            }

            // No match found, reset current input and timer
            currentBinaryInput = "";
            timer = 0.0f;
        }

        // Check if time limit has been reached
        if (currentBinaryInput.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer > 20.0f)
            {
                currentBinaryInput = "";
                timer = 0.0f;
            }
        }
    }

    public char? GetMatchedKey(string binaryInput)
    {
        // Check for a match in lowercase and uppercase dictionaries
        foreach (KeyValuePair<char, string> kvp in lowercaseBinaryDict)
        {
            if (kvp.Value == binaryInput)
            {
                Debug.Log("Match found: " + kvp.Key);
                return kvp.Key;
            }
        }
        foreach (KeyValuePair<char, string> kvp in uppercaseBinaryDict)
        {
            if (kvp.Value == binaryInput)
            {
                Debug.Log("Match found: " + kvp.Key);
                return kvp.Key;
            }
        }

        // No match found
        return null;
    }
}
