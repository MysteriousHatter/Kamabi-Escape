using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//This script handles logic for all menu buttons

public class MenuButton : MonoBehaviour, ISelectHandler
{
    public MainMenuManager mainMenuManager;
    public PauseMenu pauseMenu;
    public ButtonType menuButtonType;
    public bool isInMainMenu;
    public bool isInPauseMenu;

    //if this is a level select button which level should it load
    public int levelIndex;

    public LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        if(isInMainMenu)
        {
            mainMenuManager = GameObject.FindWithTag("MainMenuManager").GetComponent<MainMenuManager>();
            levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        }
        else if(isInPauseMenu)
        {
            pauseMenu = GameObject.FindWithTag("PauseMenu").GetComponent<PauseMenu>();
            pauseMenu.ResumeGame();
        }
    }


    // Update is called once per frame
    void Update()
    {
      //  if(this.gameObject.GetComponent<Button>().OnSelect())
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(gameObject.GetComponent<MenuButton>().menuButtonType == ButtonType.LevelSelect) {
            Debug.Log("Button selected: " + gameObject.name);
             }
    }

    //We use a switch statement driven by enums to choose the correct behavior of our menu buttons
    public void OnButtonPress()
    {
        switch(menuButtonType)
        {
            case ButtonType.Credits:
                CreditButtonEvent();
                break;

            case ButtonType.Exit:
                ExitButtonEvent();
                break;

            case ButtonType.Tutorial:
                TutorialButtonEvent();
                break;

            case ButtonType.LevelWindow:
                LevelWindowButtonEvent();
                break;

            case ButtonType.Play:
                PlayButtonEvent();
                break;

            case ButtonType.Settings:
                SettingsButtonEvent();
                break;

            case ButtonType.LevelSelect:
                LevelSelectButtonEvent(levelIndex);
                break;
        }
    }

    private void CreditButtonEvent()
    {
        mainMenuManager.ShowMenuWindow(0);
    }
    private void ExitButtonEvent()
    {
        Application.Quit();
    }
    private void TutorialButtonEvent()
    {
        if(isInMainMenu)
        {
            mainMenuManager.ShowMenuWindow(1);
        }
        else if (isInPauseMenu)
        {
            pauseMenu.ShowMenuWindow(0);
        }
        
    }
    private void LevelWindowButtonEvent()
    {
        mainMenuManager.ShowMenuWindow(2);
    }
    private void LevelSelectButtonEvent(int value)
    {
        levelManager.LoadScene(value);
    }
    private void PlayButtonEvent()
    {
        levelManager.LoadCurrentLevel();
    }
    private void SettingsButtonEvent()
    {
        if (isInMainMenu)
        {
            mainMenuManager.ShowMenuWindow(3);
        }
        else if (isInPauseMenu)
        {
            pauseMenu.ShowMenuWindow(1);
        }
    }
}



