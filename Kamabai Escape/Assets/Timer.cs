using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float time;
    private bool timerIsRunning = false;
    public TextMeshProUGUI timeText;

    private void Start()
    {
        // Start the timer automatically
        //timerIsRunning = true;
        timeText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Only update the time if the timer is running
        if (timerIsRunning)
        {
            if (time > 0)
            {
                time += Time.deltaTime;
                DisplayTime(time);
            }
            else
            {
                Debug.Log("Time has run out!");
                time = 0;
                timerIsRunning = false;
            }
        }
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    public void StartTimer()
    {
        timerIsRunning = true;
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}