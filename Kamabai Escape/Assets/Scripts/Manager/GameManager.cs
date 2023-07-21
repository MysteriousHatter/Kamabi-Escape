using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq; // Include this to use ToList()
//using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPoint;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float respawnTime;
    [SerializeField]
    private GameObject deathUI;
    [SerializeField]
    private Goal goal;
    public LevelManager levelManager;
    public int currentLevel;
    private float respawnTimeStart;
    private bool respawn;
    public LevelResult timeResult => FindObjectOfType<LevelResult>();

    //private CinemachineVirtualCamera CVC;

    private void Start()
    {
        //CVC = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
 
    }

    private void Update()
    {
        CheckRespawn();
        EnableDeathUI();
    }

    private void EnableDeathUI()
    {
        if(!player.active)
        {
            deathUI.SetActive(true);
        }
        else
        {
            deathUI.SetActive(false);
        }
    }

    public void Respawn()
    {
        respawnTimeStart = Time.time;
        respawn = true;
    }

    private void CheckRespawn()
    {
        if(Time.time >= respawnTimeStart + respawnTime && respawn)
        {

            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;
            player.SetActive(true);
            player.GetComponent<PlayerInput>().enabled = true;
            player.GetComponent<Player>().time.StartTimer();
            // CVC.m_Follow = playerTemp.transform;
            respawn = false;
        }
    }

    public void NextLevelWasLoaded(int level)
    {
        Debug.Log("Load Level " +  level);
       // levelManager.ActivateLevels(level);
        levelManager.LoadScene(level);
    }


    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        //if (levelManager.currentLevel < levelManager.highestLevel) {
        //    levelManager.currentLevel = currentLevel; 
        //}
        //else { levelManager.currentLevel = 0; }
        //if(goal.levelBeat && levelManager.currentLevel < levelManager.highestLevel) {
           
        //    Debug.Log("gOAL bEATEN " + PlayerPrefs.GetInt("levelAt"));
        //    //PlayerPrefs.SetInt("levelAt", levelManager.currentLevel);
        //    //if (levelManager.currentLevel == PlayerPrefs.GetInt("levelAt"))
        //    //{
        //    //    PlayerPrefs.SetInt("levelAt", levelManager.currentLevel);
        //    //}
        //}
        //else { levelManager.currentLevel = currentLevel++;}

        levelManager.SaveData();
        SceneManager.LoadScene(0);
    }


}
