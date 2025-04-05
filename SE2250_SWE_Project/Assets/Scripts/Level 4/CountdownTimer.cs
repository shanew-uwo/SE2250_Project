using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 60f;
    public TMP_Text timerText;

    void Update()
    {
        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(0f, timeRemaining);
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = seconds.ToString();
        
        // Change color to red when under 10 seconds
        if (seconds <= 10)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
    }
}