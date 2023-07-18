using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool isGamePaused = false;
    public GameObject pauseMenuObject;
    public GameObject[] menuWindows;
    public int currentLevel;
    public LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        isGamePaused = false;
        ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        //TogglePauseGame();
    }

    public void ShowMenuWindow(int index)
    {
        //Hide all menu windows
        foreach (GameObject obj in menuWindows)
        {
            obj.SetActive(false);
        }

        //Show the appropriate menu window
        menuWindows[index].SetActive(true);
    }

    public void TogglePauseGame()
    {
        if (isGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
        pauseMenuObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        pauseMenuObject.SetActive(false);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        levelManager.currentLevel = currentLevel;
        levelManager.SaveData();
        SceneManager.LoadScene(0);
    }

}
