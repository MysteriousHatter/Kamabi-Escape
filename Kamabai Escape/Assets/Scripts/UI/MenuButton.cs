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

    // Start is called before the first frame update
    void Start()
    {
        if(isInMainMenu)
        {
            mainMenuManager = GameObject.FindWithTag("MainMenuManager").GetComponent<MainMenuManager>();
        }
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

            case ButtonType.Level:
                LevelButtonEvent();
                break;

            case ButtonType.Play:
                PlayButtonEvent();
                break;

            case ButtonType.Settings:
                SettingsButtonEvent();
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
    private void LevelButtonEvent()
    {
        mainMenuManager.ShowMenuWindow(2);
    }
    private void PlayButtonEvent()
    {
        LevelManager.Instance.LoadCurrentLevel();
    }
    private void SettingsButtonEvent()
    {
        mainMenuManager.ShowMenuWindow(3);
    }
}



