using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Goal : MonoBehaviour
{
    public LevelResult levelResult;  // Reference to the LevelResult script
    public GameObject resultUI;      // Reference to the UI object
    public LevelManager levelManager;
    public bool levelBeat {  get; set; }

    private void Start()
    {
        levelBeat = false;
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Assuming you have a "Player" tag on your player
        {
            this.gameObject.SetActive(false);
            other.gameObject.GetComponent<Player>().time.StopTimer();
            levelBeat = true;
            levelManager.currentLevel++;
            int nextLevel = levelManager.currentLevel > 4 ? levelManager.currentLevel = 2 : levelManager.currentLevel + 1;
            PlayerPrefs.SetInt("levelAt", nextLevel++);
            other.GetComponent<Player>().src.PlayOneShot(other.GetComponent<Player>().candyCollectedSFX);
            other.GetComponent<Player>().src.clip = other.GetComponent<Player>().candyCollectedSFX;
            other.GetComponent<Player>().src.Play();
            other.gameObject.GetComponent<PlayerInput>().enabled = false;
            levelResult.CheckPlayerResult();  // Check the player's result
            resultUI.SetActive(true);  // Activate the UI
        }
    }
}
