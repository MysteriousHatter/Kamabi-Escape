using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//This script handles navigation between levels and/or scenes

public class LevelManager : MonoBehaviour
{
    public DefaultInputActions inputActions;

    public static LevelManager Instance { get; private set; }

    string scoreKey;
    public int currentLevel;
    public int highestLevel;
    public int currentScore;


    

    public int[] highScores; //the high score for each level, where the value is the score and the index is the level.
                             //the length of this array should be the number of levels in the game
    public GameObject[] levelButtons; // Assign in inspector


    //Whenever you start a level score should be set to zero at the beginning of the level - TODO

    private void Awake()
    {

        if (Instance == null)
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
        highestLevel = 4;
        currentLevel = 1;
    }

    // Update is called once per frame
    void Update()
    {
        DeletePlayerData();
    }

    private static void DeletePlayerData()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Deleete data");

            PlayerPrefs.DeleteAll();
        }
    }

    public void ChangeScore(int value)
    {
        currentScore = currentScore + value;
    }

    public void SaveData()
    {
        //set the high score for the current level
        highScores[currentLevel] = currentScore;
        scoreKey = "Score" + currentLevel;
        PlayerPrefs.SetInt(scoreKey, currentScore);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("HighestLevel", highestLevel);
       // ActivateLevels(PlayerPrefs.GetInt("CurrentLevel", currentLevel));
        PlayerPrefs.Save();
    }


    public void SaveData(int score, int level)
    {
        //set the high score for the current level
        highScores[level] = score;
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
        highestLevel = PlayerPrefs.GetInt("HighestLevel", 4);
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
        //Everytime you beat a level you need to access level manager and you need to let it know the highest level should equate to that level
        if(isLevelUnlocked(value))
        {
            currentLevel = value;
            SceneManager.LoadScene(value);

            if (currentLevel > PlayerPrefs.GetInt("levelAt"))
            {
                PlayerPrefs.SetInt("levelAt", currentLevel);
            }
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

