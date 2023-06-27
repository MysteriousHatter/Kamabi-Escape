using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles logic for all menu buttons

public class MenuButton : MonoBehaviour
{
    public MainMenuManager mainMenuManager;
    public ButtonType menuButtonType;
    public bool isInMainMenu;

    //if this is a level select button which level should it load
    public int levelIndex;

    public LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        if(isInMainMenu)
        {
            mainMenuManager = GameObject.FindWithTag("MainMenuManager").GetComponent<MainMenuManager>();
        }

        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
    }


    // Update is called once per frame
    void Update()
    {
        
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
        mainMenuManager.ShowMenuWindow(1);
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
        mainMenuManager.ShowMenuWindow(3);
    }
}



