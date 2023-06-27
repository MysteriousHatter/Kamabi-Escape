using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This script handles navigation between levels and/or scenes

public class LevelManager : MonoBehaviour
{
    
    public static LevelManager Instance { get; private set; }

    public int currentLevel;
    public bool[] isSceneUnlocked;

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
        currentLevel = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //We track whether a scene is unlocked through an array of bools.
    //The index of the array corresponds to the index of the scene
    public void UnlockScene(int index)
    {
        isSceneUnlocked[index] = true;
    }
    public void LockScene(int index)
    {
        isSceneUnlocked[index] = false;
    }

    public void LoadScene(int value)
    {
        //if scene is unlocked, load scene and update current scene int
        if(isSceneUnlocked[value])
        {
            currentLevel = value;
            SceneManager.LoadScene(value);
        }
    }

    public void LoadCurrentLevel()
    {
        //if scene is unlocked, load scene and update current scene int
        if (isSceneUnlocked[currentLevel])
        {
            SceneManager.LoadScene(currentLevel);
        }
    }
}
