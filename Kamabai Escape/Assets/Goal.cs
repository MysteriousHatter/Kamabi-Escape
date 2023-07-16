using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public LevelResult levelResult;  // Reference to the LevelResult script
    public GameObject resultUI;      // Reference to the UI object

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Assuming you have a "Player" tag on your player
        {
            this.gameObject.SetActive(false);
            other.gameObject.GetComponent<Player>().time.StopTimer();
            levelResult.CheckPlayerResult();  // Check the player's result
            resultUI.SetActive(true);  // Activate the UI
        }
    }
}
