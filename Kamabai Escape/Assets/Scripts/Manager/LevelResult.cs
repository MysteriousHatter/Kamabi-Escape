using UnityEngine;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelResult : MonoBehaviour
{
    public LevelScoreData levelScoreData; // The data object that stores the level times and scores
    public float playerTime;              // The player's current time
    public int playerScore;               // The player's current score
    public TextMeshProUGUI timeText;      // TextMeshPro element to display the player's time
    private Player player;
    public Image rankImage;               // UI Image to display the player's rank
    public Sprite[] rankSprites;          // Array of Sprites for different ranks

    // This function checks the player's current time against the level times and scores


    public void CheckPlayerResult()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            playerTime = player.time.time;
            // Sort the LevelDataList by time, so that we can find the correct score for the player's time
            levelScoreData.LevelDataList.Sort((a, b) => a.Time.CompareTo(b.Time));

            for (int i = 0; i < levelScoreData.LevelDataList.Count; i++)
            {
                // If the player's time is less than or equal to the time for this level data
                if (playerTime <= levelScoreData.LevelDataList[i].Time)
                {
                    // Reward the player with the score for this level data
                    playerScore = levelScoreData.LevelDataList[i].Score;

                    Debug.Log($"Player reached the record time! They are awarded with {playerScore} points.");

                    // Display the player's time in the UI
                    DisplayPlayerTime();

                    // Set the player's rank image
                    SetRankImage(i);

                    return; // We found the correct level data for the player's time, so we can exit the function now
                }
            }

            Debug.Log("Player did not reach any record times.");
        }
    }

    void DisplayPlayerTime()
    {
        float minutes = Mathf.FloorToInt(playerTime / 60);
        float seconds = Mathf.FloorToInt(playerTime % 60);
        float milliseconds = (playerTime % 1) * 1000;

        // Format and display the time
        timeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    void SetRankImage(int rank)
    {
        // Check if the rank is within the bounds of the array
        Debug.Log("Rank Number " + rank);
        if (rank >= 0 && rank < rankSprites.Length)
        {
            rankImage.sprite = rankSprites[rank];
        }
        else
        {
            Debug.LogWarning("Rank is out of bounds of rankSprites array.");
        }
    }
}