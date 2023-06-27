using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this script holds the logic and references to all Main Menu UI objects

public class MainMenuManager : MonoBehaviour
{
    public GameObject[] menuWindows;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMenuWindow(int index)
    {
        //Hide all menu windows
        foreach(GameObject obj in menuWindows)
        {
            obj.SetActive(false);
        }

        //Show the appropriate menu window
        menuWindows[index].SetActive(true);
    }
}
