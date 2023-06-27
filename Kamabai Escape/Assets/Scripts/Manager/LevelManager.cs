using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This script handles navigation between levels and/or scenes

public class LevelManager : MonoBehaviour
{
    
    public static LevelManager Instance { get; private set; }

    string scoreKey;
    public int currentLevel;
    public int highestLevel;

    public int[] highScores; //the high score for each level, where the value is the score and the index is the level.
    //the length of this array should be the number of levels in the game
    

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        highestLevel = 1;
        currentLevel = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveData(int score, int level)
    {
        scoreKey = "Score" + currentLevel;
        PlayerPrefs.SetInt(scoreKey, score);
        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.SetInt("HighestLevel", highestLevel);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        for (int i = 0; i < highScores.Length; i++)
        {
            scoreKey = "Score" + i + 1;
            highScores[i] = PlayerPrefs.GetInt(scoreKey);
        }
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        highestLevel = PlayerPrefs.GetInt("HighestLevel", 1);
    }

    public bool isLevelUnlocked(int level)
    {
        //the player will have access to the level after their highest level and all lower levels.
        if (level > highestLevel + 1) return false;

        return true;
    }

    public void LoadScene(int value)
    {
        //if scene is unlocked, load scene and update current scene int
        if(isLevelUnlocked(value))
        {
            currentLevel = value;
            SceneManager.LoadScene(value);
        }
    }

    public void LoadCurrentLevel()
    {
        //if scene is unlocked, load scene and update current scene int
        if (isLevelUnlocked(currentLevel))
        {
            SceneManager.LoadScene(currentLevel);
        }

        if(currentLevel > highestLevel)
        {
            highestLevel = currentLevel;
        }
    }
}
