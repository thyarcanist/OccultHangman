using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour
{
    // Achievements to Earn
    // Unlocking these achievements nets rewards.

    bool finishedTenBinaryGames = false;
    bool finishedTenNormalGames = false;
    bool finishedTenNumpadGames = false;
    bool completedSingularThemeDictionary = false;
    bool successfullyGuessedBinaryLetter = false;
    bool completedTheMainStory = false;
    bool occultInitiate = false; // completed upon making a profile
    bool accessedTheUnknown = false; // compelted tethered game connection
    // Fully completing one theme will unlock 'story' it's intended as a one-shot. No saves or anything.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
