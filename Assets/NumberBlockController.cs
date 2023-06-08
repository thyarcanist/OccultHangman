using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberBlockController : MonoBehaviour
{

    // This is assigned to individual block representation of the numberpad

    public NumBlockType numBlockType;
    public enum NumBlockType
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Asterisk,
        Zero,
        Hash
    }

}
