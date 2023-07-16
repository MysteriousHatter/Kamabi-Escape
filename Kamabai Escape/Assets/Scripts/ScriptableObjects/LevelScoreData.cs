using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelScoreData", menuName = "ScriptableObjects/LevelScoreData", order = 1)]
public class LevelScoreData : ScriptableObject
{
    public List<LevelData> LevelDataList = new List<LevelData>();
}
