using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySettings : MonoBehaviour
{
    public static DifficultySettings difficultySettings;

    public static int SpawnGhostSec;
    public static int HoldSeeds;
    public static int HoldWater;

    bool gameStart;
    // Start is called before the first frame update
    void Awake()
    {
        if(!gameStart)
        {
            difficultySettings = this;
            gameStart = true;
        }
    }

}
