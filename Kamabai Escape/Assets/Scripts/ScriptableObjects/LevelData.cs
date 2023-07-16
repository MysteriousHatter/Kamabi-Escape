using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public float Time; // Time for the level in seconds
    public int Score;  // Score for the level

    public LevelData(float time, int score)
    {
        Time = time;
        Score = score;
    }
}
